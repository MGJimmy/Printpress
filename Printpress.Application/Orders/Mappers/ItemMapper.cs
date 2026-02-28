using Printpress.Domain;

namespace Printpress.Application
{
    internal class ItemMapper (ItemDetailsMapper itemDetailsMapper) : BaseMapper<OrderItem, ItemUpsertDTO>
    {
        public override OrderItem MapFromDestinationToSource(ItemUpsertDTO destinationEntity)
        {
            return new OrderItem
            {
                Id = destinationEntity.ObjectState == TrackingState.Added ? 0 : destinationEntity.Id,
                Name = destinationEntity.Name,
                Price = destinationEntity.Price,
                Quantity = destinationEntity.Quantity,
                ObjectState = destinationEntity.ObjectState,
                IsDeleted = destinationEntity.ObjectState == TrackingState.Deleted,
                Details = itemDetailsMapper.MapFromDestinationToSource(destinationEntity.Details)
            };

        }

        public override ItemUpsertDTO MapFromSourceToDestination(OrderItem sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
