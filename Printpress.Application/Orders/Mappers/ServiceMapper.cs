using Printpress.Domain;

namespace Printpress.Application
{
    internal class ServiceMapper : BaseMapper<Service, ServiceDto>
    {
        public ServiceMapper(IGuidGenerator guidGenerator) : base(guidGenerator)
        {
            
        }
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
                ServiceCategoryId = sourceEntity.ServiceCategoryId,
                ServiceCategoryCode = sourceEntity.ServiceCategory?.Code,
                ServiceCategoryName = sourceEntity.ServiceCategory?.Name
            };
        }

        public Service MapFromDestinationToSource(ServiceUpsertDto destinationEntity)
        {
            return new Service
            {
                Name = destinationEntity.Name,
                Price = destinationEntity.Price,
                ServiceCategoryId = destinationEntity.ServiceCategoryId
            };
        }

        public Service MapFromDestinationToSource(Guid id, ServiceUpsertDto destinationEntity)
        {
            var x = MapFromDestinationToSource(destinationEntity);
            x.Id = id;
            return x;
        }
    }
}
