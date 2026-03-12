namespace Printpress.Application;

public interface IPayrollPeriodService
{
    Task<List<PayrollPeriodDto>> GetAllAsync();
    Task<PayrollPeriodDetailsDto> GetDetailsAsync(Guid id);
    Task<PayrollPeriodDto> CreateAsync(PayrollPeriodCreateDto payload, string userId);
    Task CloseAsync(Guid id, string userId);
    bool IsPeriodClosed(Guid periodId);
}
