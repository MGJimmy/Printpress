namespace Printpress.Application;

public interface IPayrollPeriodService
{
    Task<PagedList<PayrollPeriodDto>> GetAllAsync(Paging paging);
    Task<List<PayrollPeriodDto>> GetOpenPeriodsAsync();
    Task<PayrollPeriodDetailsDto> GetDetailsAsync(Guid id);
    Task<PayrollPeriodDto> CreateAsync(PayrollPeriodCreateDto payload, string userId);
    Task CloseAsync(Guid id, string userId);
    bool IsPeriodClosed(Guid periodId);
}
