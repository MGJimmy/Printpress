using Printpress.Domain.Entities;
using Printpress.Domain.Enums;

namespace Printpress.Application;

public class OrderService(IUnitOfWork _IUnitOfWork, OrderMapper _OrderMapper) : IOrderService
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

    public async Task InsertOrder(OrderUpsertDto orderDTO)
    {

        Order order = _OrderMapper.MapFromDestinationToSource(orderDTO);

        order.Status = OrderStatusEnum.New;

        order.TotalPaid = 0;

        order.TotalPrice = CalculateOrderTotalPrice(order);

        await _IUnitOfWork.OrderRepository.AddAsync(order);

        await _IUnitOfWork.SaveChangesAsync();
    }

    private decimal? CalculateOrderTotalPrice(Order order)
    {
        decimal totalOrderPrice = 0;

        foreach (var group in order.OrderGroups)
        {
            SetGroupItemPrices(group, order.Services);

            totalOrderPrice += group.Items.Sum(i => i.Price);
        }

        return totalOrderPrice;
    }

    private void SetGroupItemPrices(OrderGroup group, List<Domain.Entities.OrderService> orderService)
    {
        var CachedServices =  new List<Service>();
        // check existing Services

        var serviceList = CachedServices.Where(s => group.Services.Exists(x => s.Id == x.ServiceId)).ToList();

        if (serviceList.Exists(x => x.ServiceCategory == ServiceCategoryEnum.Selling))
        {
            return;
        }

        var printingService = serviceList.Find(x => x.ServiceCategory == ServiceCategoryEnum.Printing);
        var staplingService = serviceList.Find(x => x.ServiceCategory == ServiceCategoryEnum.Stapling);
        var cluingService = serviceList.Find(x => x.ServiceCategory == ServiceCategoryEnum.Clueing);
        var cuttingService = serviceList.Find(x => x.ServiceCategory == ServiceCategoryEnum.Cutting);
        
        // Validate incoming data to be .... ???/???/???

        foreach (var item in group.Items)
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
                return orderService.Find(x => service.Id == x.ServiceId).Price.GetValueOrDefault();
            }
        }
    }

    private decimal CalculatePrintingServicePrice(Item item, decimal price)
    {
        string stringNoOfPages =  item.Details.Find(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPages).Value;
        var noOfPages = string.IsNullOrEmpty(stringNoOfPages) ? 1 : int.Parse(stringNoOfPages);

        string stringNoOfPrintingFaces = item.Details.Find(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPrintingFaces).Value;
        var noOfPrintingFaces = string.IsNullOrEmpty(stringNoOfPrintingFaces) ? 1 : int.Parse(stringNoOfPrintingFaces);

        return price * noOfPages / noOfPrintingFaces;
    }
}
