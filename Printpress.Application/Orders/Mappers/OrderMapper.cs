using Printpress.Domain;

namespace Printpress.Application;

internal class OrderMapper(
    OrderGroupMapper _orderGroupMapper,
    OrderServiceMapper _orderServiceMapper,
    OrderSellingItemMapper _orderSellingItemMapper,
    IGuidGenerator guidGenerator) : BaseMapper<Order, OrderUpsertDto>(guidGenerator)
{
    public override Order MapFromDestinationToSource(OrderUpsertDto destinationEntity)
    {
        var order = new Order
        {
            Id = destinationEntity.ObjectState == TrackingState.Added ? _guidGenerator.NewGuid() : destinationEntity.Id,
            Name = destinationEntity.Name,
            ClientId = destinationEntity.ClientId,
            IsZeroOrder = destinationEntity.IsZeroOrder,
            ObjectState = destinationEntity.ObjectState

        };

        order.OrderGroups = _orderGroupMapper.MapFromDestinationToSource(destinationEntity.OrderGroups);
        order.Services = _orderServiceMapper.MapFromDestinationToSource(destinationEntity.OrderServices).ToList();
        order.SellingItems = _orderSellingItemMapper.MapFromDestinationToSource(destinationEntity.SellingItems).ToList();

        return order;
    }

    public override OrderUpsertDto MapFromSourceToDestination(Order sourceEntity)
    {
        throw new NotImplementedException();
    }


    public OrderSummaryDto MapToOrderSummeryDto(Order order)
    {

        var dto = new OrderSummaryDto();
        dto.Id = order.Id;
        dto.OrderName = order.Name;
        dto.ClientName = order.Client.Name;
        dto.TotalAmount = order.TotalPrice;
        dto.PaidAmount = order.TotalPaid;
        dto.OrderStatus = order.Status;
        dto.CreatedAt = order.CreatedAt;
        dto.IsZeroOrder = order.IsZeroOrder;

        return dto; ;

    }
    public PagedList<OrderSummaryDto> MapToOrderSummeryDto(PagedList<Order> orders)
    {

        return new PagedList<OrderSummaryDto>(
         orders.Items.Select(x => MapToOrderSummeryDto(x)).ToList(),
         orders.TotalCount,
         orders.PageNumber,
         orders.PageSize
         );

    }


}
