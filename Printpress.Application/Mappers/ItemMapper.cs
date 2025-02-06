using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class ItemMapper : BaseMapper<Item, ItemDTO>
    {
        public override Item MapFromDestinationToSource(ItemDTO destinationEntity)
        {
            return new Item
            {
                Id = destinationEntity.Id,
                Name = destinationEntity.Name,
                OrderGroupId = destinationEntity.GroupId,
                Price = destinationEntity.Price,
                Quantity = destinationEntity.Quantity
            };
        }

        public override ItemDTO MapFromSourceToDestination(Item sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
