using Printpress.Domain.Entities;

namespace Printpress.Application;

public class ClientService(IUnitOfWork _unitOfWork, ClientMapper _clientMapper) : IClientService
{
    public async Task<ClientDto> AddAsync(ClientUpsertDto payload)
    {
        // Make validation

        var client = await _unitOfWork.ClientRepository.AddAsync(_clientMapper.MapFromDestinationToSource(payload));


        await _unitOfWork.SaveChangesAsync();

        return _clientMapper.MapFromSourceToDestination(client);
    }

    public async Task<ClientDto> UpdateAsync(int id, ClientUpsertDto payload)
    {
        // Make validation

        if(!_unitOfWork.ClientRepository.Any(x=>x.Id == id))
        {
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));  
        }

        var client = _unitOfWork.ClientRepository.Update(_clientMapper.MapFromDestinationToSource(id, payload));


        await _unitOfWork.SaveChangesAsync();

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
        var entity = await _unitOfWork.ClientRepository.FindAsync(id);
        
        if (entity is null)
        {
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));
        }

        _unitOfWork.ClientRepository.Remove(entity);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PagedList<ClientDto>> GetByPage(int pageNumber, int pageSize)
    {
        Paging paging = new Paging
        {
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        PagedList<Client> pagedList = _unitOfWork.ClientRepository.All(paging);

        // check if no data returned then return no data founds

        var result = _clientMapper.MapFromSourceToDestination(pagedList);

        return await Task.FromResult(result);
    }
}
