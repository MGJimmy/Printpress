namespace Printpress.Application;

public interface IClientService
{
    Task<ClientDto> AddAsync(ClientUpsertDto payload, string userId);
    Task<ClientDto> UpdateAsync(int id, ClientUpsertDto payload, string userId);
    Task<ClientDto> GetClientById(int id);
    Task DeleteAsync(int id);
    Task<PagedList<ClientDto>> GetByPage(int pageNumber, int pageSize);
    Task<List<ClientDto>> GetAll();

}