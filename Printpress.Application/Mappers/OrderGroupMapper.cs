using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class OrderGroupMapper(ItemMapper _itemMapper, GroupServiceMapper _groupServiceMapper) : BaseMapper<OrderGroup, OrderGroupDTO>
    {
        public override OrderGroup MapFromDestinationToSource(OrderGroupDTO destinationEntity)
        {
            var group = new OrderGroup
            {
                Id = destinationEntity.Id,
                Name = destinationEntity.Name,
                OrderId = destinationEntity.OrderId
            };

            group.Items = _itemMapper.MapFromDestinationToSource(destinationEntity.Items);
            group.Services = _groupServiceMapper.MapFromDestinationToSource(destinationEntity.OrderGroupServices);

            return group;
        }

        public override OrderGroupDTO MapFromSourceToDestination(OrderGroup sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
