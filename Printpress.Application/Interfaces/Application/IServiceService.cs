namespace Printpress.Application;

public interface IServiceService
{
    Task<ServiceDto> AddAsync(ServiceUpsertDto payload);
    Task<ServiceDto> UpdateAsync(int id, ServiceUpsertDto payload);
    Task<ServiceDto> GetById(int id);
    Task DeleteAsync(int id);
    Task<List<ServiceDto>> GetAll();

}