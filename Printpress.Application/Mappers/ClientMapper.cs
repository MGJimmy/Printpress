using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class ClientMapper : BaseMapper<Client, ClientDto>
    {
        public override Client MapFromDestinationToSource(ClientDto destinationEntity)
        {
            throw new NotImplementedException();
        }

        public override ClientDto MapFromSourceToDestination(Client entity)
        {
            return new ClientDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Mobile = entity.Mobile,
                Address = entity.Address
            };
        }

        public Client MapFromDestinationToSource(int id, ClientUpsertDto destinationEntity)
        {
            var x = MapFromDestinationToSource(destinationEntity);
            x.Id = id;
            return x;
        }

        public Client MapFromDestinationToSource(ClientUpsertDto destinationEntity)
        {
            return new Client
            {
                Name = destinationEntity.Name,
                Mobile = destinationEntity.Mobile,
                Address = destinationEntity.Address
            };
        }
    }
}
