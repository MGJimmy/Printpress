namespace Printpress.Application;

public class ClientService(IUnitOfWork _unitOfWork, ClientMapper _clientMapper) : IClientService
{
    public async Task<ClientDto> AddAsync(ClientUpsertDto payload)
    {
        // Make validation

        var client = await _unitOfWork.ClientRepository.AddAsync(_clientMapper.MapFromDestinationToSource(payload));


        _unitOfWork.SaveChanges();

        return _clientMapper.MapFromSourceToDestination(client);
    }

    public async Task<ClientDto> UpdateAsync(int id, ClientUpsertDto payload)
    {
        // Make validation

        var client = await _unitOfWork.ClientRepository.UpdateAsync(id, _clientMapper.MapFromDestinationToSource(id, payload));


        _unitOfWork.SaveChanges();

        return _clientMapper.MapFromSourceToDestination(client);
    }


    public async Task<ClientDto> GetClientById(int id)
    {

        var client = await _unitOfWork.ClientRepository.FindAsync(id);

        if (client is null) throw new ValidationExeption("Client not found");

        return _clientMapper.MapFromSourceToDestination(client);
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.ClientRepository.Remove(id);

        _unitOfWork.SaveChanges();
    }
}
