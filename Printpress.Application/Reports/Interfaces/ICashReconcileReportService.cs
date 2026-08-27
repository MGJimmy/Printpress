namespace Printpress.Application;

public interface ICashReconcileReportService
{
    Task<CashReconcileReportDto> GetReportAsync(
        Guid? cashAccountId,
        DateTime? dateFrom,
        DateTime? dateTo);
}
