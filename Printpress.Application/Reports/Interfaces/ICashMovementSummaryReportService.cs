namespace Printpress.Application;

public interface ICashMovementSummaryReportService
{
    Task<CashMovementSummaryReportDto> GetReportAsync(
        Guid? cashAccountId,
        DateTime? dateFrom,
        DateTime? dateTo);
}
