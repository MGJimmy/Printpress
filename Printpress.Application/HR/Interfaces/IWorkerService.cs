namespace Printpress.Application;

public interface IWorkerService
{
    Task<List<WorkerDto>> GetAllAsync();
    Task<WorkerDetailsDto> GetDetailsAsync(Guid id, DateTime? productionDateFrom, DateTime? productionDateTo);
    Task<WorkerDto> CreateAsync(WorkerCreateDto payload, string userId);
    Task<WorkerDto> UpdateAsync(WorkerUpdateDto payload, string userId);
    Task DeactivateAsync(Guid id, string userId);
}
