using Printpress.Domain.Entities;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    internal static class MapperExtension
    {
        public static List<T2> MapAsList<T1, T2>(this IEnumerable<T1> list, Func<T1, T2> singleMapperFunc)
        {
            return list == null ? new List<T2>() : [.. list.Select(x => singleMapperFunc(x))];
        }
        public static OrderDto MapToOrderDTO(this Order order)
        {
            var orderDto = new OrderDto
            {
                Id = order.Id,
                Name = order.Name,
                ClientId = order.ClientId,
                ClientName = order.Client.Name,
                TotalPaid = order.TotalPaid,
                TotalPrice = order.TotalPrice,
                OrderServices = order.Services.MapAsList(MapToOrderServiceDTO),
                OrderGroups = order.OrderGroups.MapAsList(MapToOrderGroupDTO),
                ObjectState = TrackingState.Unchanged
            };

            return orderDto;
        }
        public static OrderGroupDTO MapToOrderGroupDTO(this OrderGroup OrderGroup)
        {
            var orderGroupDTO = new OrderGroupDTO
            {
                Id = OrderGroup.Id,
                Name = OrderGroup.Name,
                DeliveredFrom = OrderGroup.DeliveryName,
                DeliveredTo = OrderGroup.ReceiverName,
                DeliveryDate = OrderGroup.DeliveryDate,
                DeliveryNotes =OrderGroup.DeliveryNotes,
                OrderId = OrderGroup.OrderId,
                Status = OrderGroup.Status,
                OrderGroupServices = OrderGroup.OrderGroupServices.MapAsList(MapToGroupServiceDTO),
                Items = OrderGroup.Items.MapAsList(MapToItemDTO),
                ObjectState = TrackingState.Unchanged
            };

            return orderGroupDTO;
        }
        public static OrderServiceDTO MapToOrderServiceDTO(this OrderService orderService)
        {
            var orderServiceDTO = new OrderServiceDTO
            {
                Id = orderService.Id,
                OrderId = orderService.OrderId,
                Price = orderService.Price,
                ServiceId = orderService.ServiceId,
                //ObjectState = ObjectState.Unchanged
            };

            return orderServiceDTO;
        }
        public static OrderGroupServiceDTO MapToGroupServiceDTO(this OrderGroupService groupService)
        {
            var orderGroupServiceDTO = new OrderGroupServiceDTO
            {
                Id = groupService.Id,
                ServiceId = groupService.ServiceId,
                OrderGroupId = groupService.OrderGroupId,
                ServiceName = groupService.Service?.Name,
                ObjectState = TrackingState.Unchanged
            };

            return orderGroupServiceDTO;
        }
        public static ItemDTO MapToItemDTO(this Item item)
        {
            var itemDTO = new ItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                GroupId = item.OrderGroupId,
                Price = item.Price,
                Quantity = item.Quantity,
                Details = item.Details.MapAsList(MapToItemDetailsDTO),
                ObjectState = TrackingState.Unchanged
            };

            return itemDTO;
        }

        public static ItemDetailsDTO MapToItemDetailsDTO(this ItemDetails itemDetails)
        {
            var itemDTO = new ItemDetailsDTO
            {
                Id = itemDetails.Id,
                ItemId = itemDetails.ItemId,
                Value = itemDetails.Value,
                Key = itemDetails.ItemDetailsKey,
                ObjectState = TrackingState.Unchanged
            };

            return itemDTO;
        }
        public static OrderMainDataDto MapToOrderMainDataDto(this Order order)
        {
            var orderMainDataDto = new OrderMainDataDto
            {
                Name = order.Name,
                ClientName = order.Client.Name,
                TotalPaid = order.TotalPaid,
                TotalPrice = order.TotalPrice
            };

            return orderMainDataDto;
        }
    }
}
