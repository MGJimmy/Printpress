namespace Printpress.Application;

public class ClientService(IUnitOfWork _IUnitOfWork, ClientMapper _CientMapper) : IClientService
{
    public async Task<ClietntDto> GetClientById(int id)
    {

        var client = await _IUnitOfWork.ClientRepository.FindAsync(id);

        if (client is null) throw new ValidationExeption("Client not found");

        return _CientMapper.MapFromSourceToDestination(client);
    }
}
