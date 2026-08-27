namespace Printpress.Application;

public interface ICashFlowReportService
{
    Task<CashFlowReportDto> GetReportAsync(Guid? cashAccountId, DateTime? dateFrom, DateTime? dateTo);
}
