using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public static class MapperExtension
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
                ClientName=order.Client.Name,
                TotalPaid = order.TotalPaid,
                TotalPrice = order.TotalPrice,
                OrderServices = order.Services.MapAsList(MapToOrderServiceDTO),
                OrderGroups = order.OrderGroups.MapAsList(MapToOrderGroupDTO),
            };

            return orderDto;
        }
        public static OrderGroupDTO MapToOrderGroupDTO(this OrderGroup orderService)
        {
            var orderGroupDTO = new OrderGroupDTO
            {
                Id = orderService.Id,
                Name = orderService.Name,
                OrderId = orderService.OrderId,
                OrderGroupServices = orderService.Services.MapAsList(MapToGroupServiceDTO),
                Items = orderService.Items.MapAsList(MapToItemDTO)
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
                ServiceId = orderService.ServiceId
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
                //ServiceName = groupService.,
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
                Quantity = item.Quantity
            };

            return itemDTO;
        }
    }
}
