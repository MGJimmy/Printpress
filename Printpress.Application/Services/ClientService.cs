﻿using Printpress.Domain.Entities;

namespace Printpress.Application;

internal sealed class ClientService(IUnitOfWork _unitOfWork, ClientMapper _clientMapper) : IClientService
{
    public async Task<ClientDto> AddAsync(ClientUpsertDto payload, string userId)
    {
        // Make validation

        var client = await _unitOfWork.ClientRepository.AddAsync(_clientMapper.MapFromDestinationToSource(payload));


        await _unitOfWork.SaveChangesAsync(userId);

        return _clientMapper.MapFromSourceToDestination(client);
    }

    public async Task<ClientDto> UpdateAsync(int id, ClientUpsertDto payload, string userId)
    {
        // Make validation

        if (!_unitOfWork.ClientRepository.Any(x => x.Id == id))
        {
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));
        }

        var client = _unitOfWork.ClientRepository.Update(_clientMapper.MapFromDestinationToSource(id, payload));


        await _unitOfWork.SaveChangesAsync(userId);

        return _clientMapper.MapFromSourceToDestination(client);
    }

    public async Task<ClientDto> GetClientById(int id)
    {

        var client = await _unitOfWork.ClientRepository.FindAsync(id);

        if (client is null) throw new ValidationExeption("Client not found");

        return _clientMapper.MapFromSourceToDestination(client);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var entity = await _unitOfWork.ClientRepository.FindAsync(id);

        if (entity is null)
        {
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));
        }

        _unitOfWork.ClientRepository.Remove(entity);

        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task<PagedList<ClientDto>> GetByPage(int pageNumber, int pageSize)
    {

        PagedList<Client> pagedList = await _unitOfWork.ClientRepository.AllAsync(
            new Paging(pageNumber, pageSize),
            new Sorting(nameof(Client.Name), SortingDirection.DESC)
        );

        // check if no data returned then return no data founds

        var result = _clientMapper.MapFromSourceToDestination(pagedList);

        return result;
    }

    public async Task<List<ClientDto>> GetAll()
    {
        var pagedList = await _unitOfWork.ClientRepository.AllAsync();

        var result = _clientMapper.MapFromSourceToDestination(pagedList);

        return result;

    }
}
