namespace Printpress.Application
{
    public class OrderServiceMapper : BaseMapper<Domain.Entities.OrderService, OrderServiceDTO>
    {
        public override Domain.Entities.OrderService MapFromDestinationToSource(OrderServiceDTO destinationEntity)
        {
            throw new NotImplementedException();
        }

        public override OrderServiceDTO MapFromSourceToDestination(Domain.Entities.OrderService sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
