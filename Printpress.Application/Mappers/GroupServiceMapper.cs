using Printpress.Domain.Entities;

namespace Printpress.Application
{
    internal class GroupServiceMapper : BaseMapper<OrderGroupService, OrderGroupServiceUpsertDTO>
    {
        public override OrderGroupService MapFromDestinationToSource(OrderGroupServiceUpsertDTO destinationEntity)
        {
            return new OrderGroupService
            {
                Id = destinationEntity.ObjectState == ObjectState.Added ? 0 : destinationEntity.Id,
                ServiceId = destinationEntity.ServiceId,
                State = destinationEntity.ObjectState.MapToTrackingState()
            };
        }

        public override OrderGroupServiceUpsertDTO MapFromSourceToDestination(OrderGroupService sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
