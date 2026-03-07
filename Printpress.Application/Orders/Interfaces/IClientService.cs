namespace Printpress.Application;

public interface IClientService
{
    Task<ClientDto> AddAsync(ClientUpsertDto payload, string userId);
    Task<ClientDto> UpdateAsync(Guid id, ClientUpsertDto payload, string userId);
    Task<ClientDto> GetClientById(Guid id);
    Task DeleteAsync(Guid id, string userId);
    Task<PagedList<ClientDto>> GetByPage(int pageNumber, int pageSize);
    Task<List<ClientDto>> GetAll();

}