using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain.Entities;

namespace Printpress.Application.Mappers
{
    public class ItemDetailsMapper : BaseMapper<ItemDetails, ItemDetailsUpsertDTO>
    {
        public override ItemDetails MapFromDestinationToSource(ItemDetailsUpsertDTO destinationEntity)
        {
            return new ItemDetails
            {
                Id = destinationEntity.Id,
                ItemId = destinationEntity.ItemId,
                ItemDetailsKey = destinationEntity.Key,
                Value = destinationEntity.Value
            };
        }

        public override ItemDetailsUpsertDTO MapFromSourceToDestination(ItemDetails sourceEntity)
        {
            throw new NotImplementedException();
        }
    }
}
