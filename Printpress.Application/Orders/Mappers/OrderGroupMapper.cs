using Printpress.Domain;

namespace Printpress.Application
{
    internal class OrderGroupMapper(ItemMapper _itemMapper, GroupServiceMapper _groupServiceMapper,
        IGuidGenerator guidGenerator) : BaseMapper<OrderGroup, OrderGroupUpsertDTO>(guidGenerator)
    {
        public override OrderGroup MapFromDestinationToSource(OrderGroupUpsertDTO destinationEntity)
        {
            var group = new OrderGroup
            {
                Id = destinationEntity.ObjectState == TrackingState.Added ? _guidGenerator.NewGuid() : destinationEntity.Id,
                Name = destinationEntity.Name,
                ExecutionType = destinationEntity.ExecutionType,
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
