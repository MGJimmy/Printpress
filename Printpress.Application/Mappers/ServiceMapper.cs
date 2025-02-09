using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class ServiceMapper : BaseMapper<Service, ServiceDto>
    {
        public override Service MapFromDestinationToSource(ServiceDto destinationEntity)
        {
            throw new NotImplementedException();
        }

        public override ServiceDto MapFromSourceToDestination(Service sourceEntity)
        {
            return new ServiceDto
            {
                Id = sourceEntity.Id,
                Name = sourceEntity.Name,
                Price = sourceEntity.Price,
                ServiceCategory = sourceEntity.ServiceCategory
            };
        }

        public Service MapFromDestinationToSource(ServiceUpsertDto destinationEntity)
        {
            return new Service
            {
                Name = destinationEntity.Name,
                Price = destinationEntity.Price,
                ServiceCategory = destinationEntity.ServiceCategory
            };
        }

        public Service MapFromDestinationToSource(int id, ServiceUpsertDto destinationEntity)
        {
            var x = MapFromDestinationToSource(destinationEntity);
            x.Id = id;
            return x;
        }
    }
}
