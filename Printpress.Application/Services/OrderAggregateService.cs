using Microsoft.EntityFrameworkCore;
using Printpress.Application.Validators;
using Printpress.Domain;
using Printpress.Domain.Entities;
using Printpress.Domain.Enums;
namespace Printpress.Application;

internal sealed class OrderAggregateService(IUnitOfWork _IUnitOfWork, OrderMapper _OrderMapper) : IOrderAggregateService
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

    public async Task<OrderDto> GetOrderDTOAsync(int orderId)
    {
        string[] includes = [
            $"{nameof(Order.OrderGroups)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}.{nameof(Item.Details)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}",
            $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}.{nameof(OrderGroupService.Service)}",
            $"{nameof(Order.Services)}",
            $"{nameof(Order.Client)}"];

        var order = await _IUnitOfWork.OrderRepository.FirstOrDefaultAsync((order => order.Id == orderId), false, includes);

        if (order is null) ValidationExeption.FireValidationException("order with {id} not found", orderId);

        var orderDTO = order.MapToOrderDTO();

        return orderDTO;
    }

    public async Task<OrderMainDataDto> GetOrderMainDataAsync(int orderId)
    {
        var order = await _IUnitOfWork.OrderRepository.FirstOrDefaultAsync((order => order.Id == orderId), false, nameof(Order.Client));

        if (order is null) ValidationExeption.FireValidationException("order with {id} not found", orderId);

        return order.MapToOrderMainDataDto();
    }
    public async Task InsertOrder(OrderUpsertDto orderDTO)
    {

        Order order = _OrderMapper.MapFromDestinationToSource(orderDTO);

        order.Status = OrderStatusEnum.New;

        order.TotalPaid = 0;

        order.TotalPrice = await CalculateOrderTotalPrice(order);

        await _IUnitOfWork.OrderRepository.AddAsync(order);

        await _IUnitOfWork.SaveChangesAsync();
    }

    private async Task<decimal?> CalculateOrderTotalPrice(Order order)
    {
        decimal totalOrderPrice = 0;

        foreach (var group in order.OrderGroups.NotDeleted())
        {
            await SetGroupItemPrices(group, order.Services);

            totalOrderPrice += group.Items.NotDeleted().Sum(i => i.Price * i.Quantity);
        }

        return totalOrderPrice;
    }

    private async Task SetGroupItemPrices(OrderGroup group, List<OrderService> orderService)
    {
        var allServices = await _IUnitOfWork.ServiceRepository.AllAsync();

        var groupServicesIds = new HashSet<int>(group.OrderGroupServices.NotDeleted().Select(d => d.ServiceId));
        var currentGroupServices = allServices.Where(s => groupServicesIds.Contains(s.Id)).ToList();

        if (currentGroupServices.Exists(x => x.ServiceCategory == ServiceCategoryEnum.Selling))
        {
            return;
        }

        var printingService = currentGroupServices.Find(x => x.ServiceCategory == ServiceCategoryEnum.Printing);
        var staplingService = currentGroupServices.Find(x => x.ServiceCategory == ServiceCategoryEnum.Stapling);
        var cluingService = currentGroupServices.Find(x => x.ServiceCategory == ServiceCategoryEnum.Clueing);
        var cuttingService = currentGroupServices.Find(x => x.ServiceCategory == ServiceCategoryEnum.Cutting);

        // Validate incoming data to be .... ???/???/???

        foreach (var item in group.Items.NotDeleted())
        {
            decimal itemPrice = 0;

            if (printingService != null)
            {
                var servicePrice = GetservicePrice(printingService);
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

            decimal GetservicePrice(Service service)
            {
                return orderService.NotDeleted().First(x => service.Id == x.ServiceId).Price.GetValueOrDefault();
            }
        }
    }

    private decimal CalculatePrintingServicePrice(Item item, decimal price)
    {
        string stringNoOfPages = item.Details.Find(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPages)?.Value;
        var noOfPages = string.IsNullOrEmpty(stringNoOfPages) ? 1 : int.Parse(stringNoOfPages);

        string stringNoOfPrintingFaces = item.Details.Find(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPrintingFaces)?.Value;
        var noOfPrintingFaces = string.IsNullOrEmpty(stringNoOfPrintingFaces) ? 1 : int.Parse(stringNoOfPrintingFaces);

        return price * noOfPages / noOfPrintingFaces;
    }

    public async Task UpdateOrder(int id,OrderUpsertDto orderDTO)
    {
        Order order = _OrderMapper.MapFromDestinationToSource(orderDTO);

        order.TotalPrice = await CalculateOrderTotalPrice(order);

        _IUnitOfWork.OrderRepository.AddOrUpdate(order);

        await _IUnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteOrder(int id)
    {
        var order = await _IUnitOfWork.OrderRepository.FirstOrDefaultAsync(o => o.Id == id);
        
        if (order is null) 
            ValidationExeption.FireValidationException("Order with ID {0} not found", id);

        _IUnitOfWork.OrderRepository.Remove(order);
        await _IUnitOfWork.SaveChangesAsync();
    }
}
