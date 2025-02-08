﻿using Printpress.Domain.Entities;

namespace Printpress.Application;

public class OrderMapper(OrderGroupMapper _orderGroupMapper, OrderServiceMapper _orderServiceMapper) : BaseMapper<Order, OrderDto>
{
    public override Order MapFromDestinationToSource(OrderDto destinationEntity)
    {
        var order = new Order
        {
            Id = destinationEntity.Id, // ??
            Name = destinationEntity.Name,
            ClientId = destinationEntity.ClientId,
            TotalPaid = destinationEntity.TotalPaid,
            TotalPrice = destinationEntity.TotalPrice
        };
        order.OrderGroups = _orderGroupMapper.MapFromDestinationToSource(destinationEntity.OrderGroups);
        order.Services = _orderServiceMapper.MapFromDestinationToSource(destinationEntity.OrderServices).ToList();

        return order;
    }

    public override OrderDto MapFromSourceToDestination(Order sourceEntity)
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
        dto.CreatedAt = DateTime.Now;

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
