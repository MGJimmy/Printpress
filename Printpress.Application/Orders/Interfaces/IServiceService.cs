namespace Printpress.Application;

public interface IServiceService
{
    Task<ServiceDto> AddAsync(ServiceUpsertDto payload, string userId);
    Task<ServiceDto> UpdateAsync(int id, ServiceUpsertDto payload, string userId);
    Task<ServiceDto> GetById(int id);
    Task DeleteAsync(int id, string userId);
    Task<List<ServiceDto>> GetAll();

}