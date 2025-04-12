using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class OrderGroupMapper(ItemMapper _itemMapper, GroupServiceMapper _groupServiceMapper) : BaseMapper<OrderGroup, OrderGroupUpsertDTO>
    {
        public override OrderGroup MapFromDestinationToSource(OrderGroupUpsertDTO destinationEntity)
        {
            var group = new OrderGroup
            {
                Id = destinationEntity.ObjectState == ObjectState.Added ? 0 : destinationEntity.Id,
                Name = destinationEntity.Name,
                State = destinationEntity.ObjectState.MapToTrackingState()
            };

            group.Items = _itemMapper.MapFromDestinationToSource(destinationEntity.Items);
            group.Services = _groupServiceMapper.MapFromDestinationToSource(destinationEntity.OrderGroupServices);

            return group;
        }

        public override OrderGroupUpsertDTO MapFromSourceToDestination(OrderGroup sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
