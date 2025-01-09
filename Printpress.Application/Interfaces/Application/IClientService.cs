namespace Printpress.Application;

public interface IClientService
{
    Task<ClientDto> AddAsync(ClientUpsertDto payload);
    Task<ClientDto> UpdateAsync(int id, ClientUpsertDto payload);
    Task<ClientDto> GetClientById(int id);
    Task DeleteAsync(int id);
    Task<PagedList<ClientDto>> GetByPage(int pageNumber, int pageSize);
}