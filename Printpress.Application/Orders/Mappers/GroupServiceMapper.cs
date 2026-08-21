using Printpress.Domain;

namespace Printpress.Application
{
    internal class GroupServiceMapper : BaseMapper<OrderGroupService, OrderGroupServiceUpsertDTO>
    {
        public GroupServiceMapper(IGuidGenerator guidGenerator) : base(guidGenerator)
        {
            
        }
        public override OrderGroupService MapFromDestinationToSource(OrderGroupServiceUpsertDTO destinationEntity)
        {
            return new OrderGroupService
            {
                Id = destinationEntity.ObjectState == TrackingState.Added ? _guidGenerator.NewGuid() : destinationEntity.Id,
                ServiceId = destinationEntity.ServiceId,
                IsCover = destinationEntity.IsCover,
                ObjectState = destinationEntity.ObjectState
            };
        }

        public override OrderGroupServiceUpsertDTO MapFromSourceToDestination(OrderGroupService sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
