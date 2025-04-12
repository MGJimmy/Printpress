using Printpress.Application.Mappers;
using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class ItemMapper (ItemDetailsMapper itemDetailsMapper) : BaseMapper<Item, ItemUpsertDTO>
    {
        public override Item MapFromDestinationToSource(ItemUpsertDTO destinationEntity)
        {
            return new Item
            {
                Id = destinationEntity.ObjectState == ObjectState.Added ? 0 : destinationEntity.Id,
                Name = destinationEntity.Name,
                Price = destinationEntity.Price,
                Quantity = destinationEntity.Quantity,
                State = destinationEntity.ObjectState.MapToTrackingState(),
                IsDeleted = destinationEntity.ObjectState == ObjectState.Deleted,
                Details = itemDetailsMapper.MapFromDestinationToSource(destinationEntity.Details)
            };

        }

        public override ItemUpsertDTO MapFromSourceToDestination(Item sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
