namespace Printpress.Application;

public interface IServiceService
{
    Task<ServiceDto> AddAsync(ServiceUpsertDto payload, string userId);
    Task<ServiceDto> UpdateAsync(Guid id, ServiceUpsertDto payload, string userId);
    Task<ServiceDto> GetById(Guid id);
    Task DeleteAsync(Guid id, string userId);
    Task<List<ServiceDto>> GetAll();
    Task DeactivateAsync(Guid id, string userId);

}