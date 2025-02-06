using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class GroupServiceMapper : BaseMapper<OrderGroupService, OrderGroupServiceDTO>
    {
        public override OrderGroupService MapFromDestinationToSource(OrderGroupServiceDTO destinationEntity)
        {
            return new OrderGroupService
            {
                Id = destinationEntity.Id,
                ServiceId = destinationEntity.ServiceId,
                OrderGroupId = destinationEntity.OrderGroupId
            };
        }

        public override OrderGroupServiceDTO MapFromSourceToDestination(OrderGroupService sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
