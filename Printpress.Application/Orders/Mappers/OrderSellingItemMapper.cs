using Printpress.Domain;

namespace Printpress.Application;

internal class OrderSellingItemMapper : BaseMapper<OrderSellingItem, OrderSellingItemUpsertDTO>
{
    public OrderSellingItemMapper(IGuidGenerator guidGenerator) : base(guidGenerator)
    {
    }

    public override OrderSellingItem MapFromDestinationToSource(OrderSellingItemUpsertDTO destinationEntity)
    {
        return new OrderSellingItem
        {
            Id = destinationEntity.ObjectState == TrackingState.Added ? _guidGenerator.NewGuid() : destinationEntity.Id,
            Name = destinationEntity.Name,
            InventoryItemId = destinationEntity.InventoryItemId,
            IsInventoryItem = destinationEntity.IsInventoryItem,
            Quantity = destinationEntity.Quantity,
            Price = destinationEntity.Price,
            ObjectState = destinationEntity.ObjectState
        };
    }

    public override OrderSellingItemUpsertDTO MapFromSourceToDestination(OrderSellingItem sourceEntity)
    {
        throw new NotImplementedException();
    }
}
