using Microsoft.EntityFrameworkCore;
using Printpress.Domain;
namespace Printpress.Application;

internal sealed class OrderAggregateService(IUnitOfWork _IUnitOfWork, OrderMapper _OrderMapper, IGuidGenerator _guidGenerator, ILocalizationService _loc) : IOrderAggregateService
{
    public async Task<PagedList<OrderSummaryDto>> GetOrderSummaryListAsync(int pageNumber, int pageSize)
    {
        string[] includes = { nameof(Order.Client) };

        var orders = await _IUnitOfWork.OrderRepository.AllAsync(
            new Paging(pageNumber, pageSize),
            new Sorting(nameof(Order.Id), SortingDirection.DESC),
            includes
        );

        return _OrderMapper.MapToOrderSummeryDto(orders);
    }

    public async Task<OrderDto> GetOrderDTOAsync(Guid orderId)
    {
        string[] includes = [
            $"{nameof(Order.OrderGroups)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}.{nameof(OrderItem.Details)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}.{nameof(OrderGroupService.Service)}",
            $"{nameof(Order.Services)}",
            $"{nameof(Order.SellingItems)}",
            $"{nameof(Order.SellingItems)}.{nameof(OrderSellingItem.InventoryItem)}",
            $"{nameof(Order.Client)}"];

        var order = await _IUnitOfWork.OrderRepository.FirstOrDefaultAsync((order => order.Id == orderId), false, includes);

        if (order is null) ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        var orderDTO = order.MapToOrderDTO();

        return orderDTO;
    }

    public async Task<OrderMainDataDto> GetOrderMainDataAsync(Guid orderId)
    {
        var order = await _IUnitOfWork.OrderRepository.FirstOrDefaultAsync((order => order.Id == orderId), false, nameof(Order.Client));

        if (order is null) ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        return order.MapToOrderMainDataDto();
    }
    public async Task InsertOrder(OrderUpsertDto orderDTO, string userId)
    {

        Order order = _OrderMapper.MapFromDestinationToSource(orderDTO);

        order.Id = _guidGenerator.NewGuid();
        order.Status = OrderStatusEnum.New;
        order.TotalPaid = 0;

        foreach (var group in order.OrderGroups ?? [])
        {
            group.Id = _guidGenerator.NewGuid();
            group.OrderId = order.Id;
            foreach (var item in group.Items ?? [])
            {
                item.Id = _guidGenerator.NewGuid();
                item.OrderGroupId = group.Id;
                foreach (var detail in item.Details ?? [])
                    detail.Id = _guidGenerator.NewGuid();
            }
            foreach (var gs in group.OrderGroupServices ?? [])
                gs.Id = _guidGenerator.NewGuid();
        }
        foreach (var os in order.Services ?? [])
            os.Id = _guidGenerator.NewGuid();

        foreach (var si in order.SellingItems ?? [])
        {
            si.Id = _guidGenerator.NewGuid();
            si.OrderId = order.Id;
        }

        order.TotalPrice = await CalculateOrderTotalPrice(order);

        await _IUnitOfWork.OrderRepository.AddAsync(order);

        await _IUnitOfWork.SaveChangesAsync(userId);
    }

    private async Task<decimal?> CalculateOrderTotalPrice(Order order)
    {
        decimal totalOrderPrice = 0;

        foreach (var group in order.OrderGroups.NotDeleted())
        {
            await SetGroupItemPrices(group, order.Services);

            totalOrderPrice += group.Items.NotDeleted().Sum(i => i.Price * i.Quantity);
        }

        totalOrderPrice += (order.SellingItems ?? []).NotDeleted().Sum(i => i.Price * i.Quantity);

        return totalOrderPrice;
    }

    private async Task SetGroupItemPrices(OrderGroup group, List<OrderService> orderService)
    {
        var allServices = await _IUnitOfWork.ServiceRepository.AllAsync(nameof(Service.ServiceCategory));

        var activeGroupServices = group.OrderGroupServices.NotDeleted().ToList();
        var groupServicesIds = new HashSet<Guid>(activeGroupServices.Select(d => d.ServiceId));
        var currentGroupServices = allServices.Where(s => groupServicesIds.Contains(s.Id)).ToList();

        if (currentGroupServices.Exists(x => x.ServiceCategory?.Code == "Selling"))
        {
            return;
        }

        ValidateGroupServices(activeGroupServices, currentGroupServices);

        var printingGroupServices = activeGroupServices
            .Where(gs => currentGroupServices.Any(s => s.Id == gs.ServiceId && s.ServiceCategory?.Code == "Printing"))
            .ToList();
        var staplingService = currentGroupServices.Find(x => x.ServiceCategory?.Code == "Stapling");
        var cluingService = currentGroupServices.Find(x => x.ServiceCategory?.Code == "Clueing");
        var cuttingService = currentGroupServices.Find(x => x.ServiceCategory?.Code == "Cutting");

        foreach (var item in group.Items.NotDeleted())
        {
            decimal itemPrice = 0;

            foreach (var printingGroupService in printingGroupServices)
            {
                var catalogService = currentGroupServices.First(s => s.Id == printingGroupService.ServiceId);
                var servicePrice = GetservicePrice(catalogService);

                if (printingGroupService.IsCover)
                    itemPrice += servicePrice;
                else
                    itemPrice += CalculatePrintingServicePrice(item, servicePrice);
            }

            if (staplingService != null)
            {
                itemPrice += GetservicePrice(staplingService);
            }

            if (cluingService != null)
            {
                itemPrice += GetservicePrice(cluingService);
            }

            if (cuttingService != null)
            {
                itemPrice += GetservicePrice(cuttingService);
            }

            item.Price = itemPrice;

            // update unchanged status to modified to update database with new calcuated item price
            item.ObjectState = item.ObjectState == TrackingState.Unchanged ? TrackingState.Modified : item.ObjectState;

            decimal GetservicePrice(Service service)
            {
                return orderService.NotDeleted().First(x => service.Id == x.ServiceId).Price.GetValueOrDefault();
            }
        }
    }

    private void ValidateGroupServices(List<OrderGroupService> groupServices, List<Service> catalogServices)
    {
        var serviceById = catalogServices.ToDictionary(s => s.Id);

        var printingServices = groupServices
            .Where(gs => serviceById.TryGetValue(gs.ServiceId, out var s) && s.ServiceCategory?.Code == "Printing")
            .ToList();

        if (printingServices.Count(gs => !gs.IsCover) > 1)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.PrintingMainDuplicate));

        if (printingServices.Count(gs => gs.IsCover) > 1)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.PrintingCoverDuplicate));

        var nonPrintingCategoryCodes = groupServices
            .Select(gs => serviceById.TryGetValue(gs.ServiceId, out var s) ? s.ServiceCategory?.Code : null)
            .Where(code => !string.IsNullOrEmpty(code) && code != "Printing" && code != "Selling")
            .GroupBy(code => code)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (nonPrintingCategoryCodes.Count > 0)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.ServiceTypeDuplicate));
    }

    private decimal CalculatePrintingServicePrice(OrderItem item, decimal price)
    {
        string stringNoOfPages = item.Details.Find(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPages)?.Value;
        var noOfPages = string.IsNullOrEmpty(stringNoOfPages) ? 1 : int.Parse(stringNoOfPages);

        string stringNoOfPrintingFaces = item.Details.Find(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPrintingFaces)?.Value;
        var noOfPrintingFaces = string.IsNullOrEmpty(stringNoOfPrintingFaces) ? 1 : int.Parse(stringNoOfPrintingFaces);

        return price * noOfPages / noOfPrintingFaces;
    }

    public async Task UpdateOrder(Guid id, OrderUpsertDto orderDTO, string userId)
    {
        Order order = _OrderMapper.MapFromDestinationToSource(orderDTO);

        order.TotalPrice = await CalculateOrderTotalPrice(order);

        _IUnitOfWork.OrderRepository.AddOrUpdate(order);

        await _IUnitOfWork.SaveChangesAsync(userId);
    }

    public async Task DeleteOrder(Guid id, string userId)
    {
        var order = await _IUnitOfWork.OrderRepository.FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            ValidationExeption.FireValidationException(_loc.Get(LocalizationKeys.Orders.OrderNotFound));

        _IUnitOfWork.OrderRepository.Remove(order);
        await _IUnitOfWork.SaveChangesAsync(userId);
    }
}
