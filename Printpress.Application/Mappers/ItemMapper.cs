using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class ItemMapper : BaseMapper<Item, ItemUpsertDTO>
    {
        public override Item MapFromDestinationToSource(ItemUpsertDTO destinationEntity)
        {
            return new Item
            {
                Id = destinationEntity.Id,
                Name = destinationEntity.Name,
                Price = destinationEntity.Price,
                Quantity = destinationEntity.Quantity
            };
        }

        public override ItemUpsertDTO MapFromSourceToDestination(Item sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
