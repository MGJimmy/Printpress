using Printpress.Domain;

namespace Printpress.Application
{
    internal class OrderServiceMapper : BaseMapper<OrderService, OrderServiceUpsertDTO>
    {
        public override OrderService MapFromDestinationToSource(OrderServiceUpsertDTO destinationEntity)
        {
            return new OrderService
            {
                Id = destinationEntity.ObjectState == TrackingState.Added ? Guid.Empty : destinationEntity.Id,
                ServiceId = destinationEntity.ServiceId,
                ObjectState = destinationEntity.ObjectState,
                Price = destinationEntity.Price
            };
        }

        public override OrderServiceUpsertDTO MapFromSourceToDestination(OrderService sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
