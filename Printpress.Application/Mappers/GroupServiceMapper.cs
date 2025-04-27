using Printpress.Domain.Entities;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    internal class GroupServiceMapper : BaseMapper<OrderGroupService, OrderGroupServiceUpsertDTO>
    {
        public override OrderGroupService MapFromDestinationToSource(OrderGroupServiceUpsertDTO destinationEntity)
        {
            return new OrderGroupService
            {
                Id = destinationEntity.ObjectState == TrackingState.Added ? 0 : destinationEntity.Id,
                ServiceId = destinationEntity.ServiceId,
                State = destinationEntity.ObjectState
            };
        }

        public override OrderGroupServiceUpsertDTO MapFromSourceToDestination(OrderGroupService sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
