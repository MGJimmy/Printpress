using Printpress.Application;

namespace Printpress.Application
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _IUnitOfWork;
        private readonly ClientMapper _CientMapper;
        public ClientService(IUnitOfWork unitOfWork, ClientMapper clientMapper)
        {
            _IUnitOfWork = unitOfWork;
            _CientMapper = clientMapper;
        }

        public async Task<ClietntDto> GetClientById(int id)
        {
            var client = await _IUnitOfWork.ClientRepository.FindAsync(id);

            if (client is null) throw new ValidationExeption("Client not found");

            return _CientMapper.MapFromSourceToDestination(client);

        }
    }
}
