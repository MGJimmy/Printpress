﻿using Printpress.Application.DTO;
using Printpress.Application.DTO.Order;
using Printpress.Domain.Entities;

namespace Printpress.Application.Mappers
{
    public class OrderMapper : BaseMapper<Order, OrderDto>
    {
        public override Order MapFromDestinationToSource(OrderDto destinationEntity)
        {
            throw new NotImplementedException();
        }

        public override OrderDto MapFromSourceToDestination(Order sourceEntity)
        {
            throw new NotImplementedException();
        }


        public OrderSummaryDto MapToOrderSummeryDto(Order order)
        {

            var dto = new OrderSummaryDto();
            dto.OrderName = order.Name;
            dto.ClientName = order.Client.Name;
            dto.TotalAmount = order.TotalPrice;
            dto.PaidAmount = order.TotalPaid;
            dto.OrderStatus = order.Status;
            //dto.CreatedAt = order.CreatedAt;

            return dto; ;

        }
        public PagedList<OrderSummaryDto> MapToOrderSummeryDto(PagedList<Order> orders)
        {

            return new PagedList<OrderSummaryDto>(
             orders.Items.Select(x =>MapToOrderSummeryDto(x)).ToList(),
             orders.TotalCount,
             orders.PageNumber,
             orders.PageSize
             );

        }


    }
}
