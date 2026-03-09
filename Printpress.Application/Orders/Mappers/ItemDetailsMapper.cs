using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain;

namespace Printpress.Application;

internal class ItemDetailsMapper : BaseMapper<OrderItemDetails, ItemDetailsUpsertDTO>
{
    public ItemDetailsMapper(IGuidGenerator guidGenerator) : base(guidGenerator)
    {
        
    }
    public override OrderItemDetails MapFromDestinationToSource(ItemDetailsUpsertDTO destinationEntity)
    {
        return new OrderItemDetails
        {
            Id = destinationEntity.ObjectState == TrackingState.Added ? _guidGenerator.NewGuid() : destinationEntity.Id,
            ItemId = destinationEntity.ItemId,
            ItemDetailsKey = destinationEntity.Key,
            Value = destinationEntity.Value,
            ObjectState = destinationEntity.ObjectState
        };
    }

    public override ItemDetailsUpsertDTO MapFromSourceToDestination(OrderItemDetails sourceEntity)
    {
        throw new NotImplementedException();
    }
}
