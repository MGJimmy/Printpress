namespace Printpress.Application
{
    public class OrderServiceMapper : BaseMapper<Domain.Entities.OrderService, OrderServiceUpsertDTO>
    {
        public override Domain.Entities.OrderService MapFromDestinationToSource(OrderServiceUpsertDTO destinationEntity)
        {
            return new Domain.Entities.OrderService
            {
                Id = destinationEntity.Id,
                ServiceId = destinationEntity.ServiceId,
                State = destinationEntity.ObjectState.MapToTrackingState()
            };
        }

        public override OrderServiceUpsertDTO MapFromSourceToDestination(Domain.Entities.OrderService sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
