using Printpress.Domain.Entities;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    internal class OrderGroupMapper(ItemMapper _itemMapper, GroupServiceMapper _groupServiceMapper) : BaseMapper<OrderGroup, OrderGroupUpsertDTO>
    {
        public override OrderGroup MapFromDestinationToSource(OrderGroupUpsertDTO destinationEntity)
        {
            var group = new OrderGroup
            {
                Id = destinationEntity.ObjectState == TrackingState.Added ? 0 : destinationEntity.Id,
                Name = destinationEntity.Name,
                ObjectState = destinationEntity.ObjectState
            };

            group.Items = _itemMapper.MapFromDestinationToSource(destinationEntity.Items);
            group.OrderGroupServices = _groupServiceMapper.MapFromDestinationToSource(destinationEntity.OrderGroupServices);

            return group;
        }

        public override OrderGroupUpsertDTO MapFromSourceToDestination(OrderGroup sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
