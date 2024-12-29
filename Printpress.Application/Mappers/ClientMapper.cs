using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class ClientMapper : BaseMapper<Client, ClietntDto>
    {
        public override Client MapFromDestinationToSource(ClietntDto destinationEntity)
        {
            throw new NotImplementedException();
        }

        public override ClietntDto MapFromSourceToDestination(Client entity)
        {
            return new ClietntDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Mobile = entity.Mobile,
                Address = entity.Address
            };
        }
    }
}
