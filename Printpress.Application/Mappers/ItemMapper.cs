using Printpress.Application.Mappers;
using Printpress.Domain.Entities;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    internal class ItemMapper (ItemDetailsMapper itemDetailsMapper) : BaseMapper<Item, ItemUpsertDTO>
    {
        public override Item MapFromDestinationToSource(ItemUpsertDTO destinationEntity)
        {
            return new Item
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

        public override ItemUpsertDTO MapFromSourceToDestination(Item sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
