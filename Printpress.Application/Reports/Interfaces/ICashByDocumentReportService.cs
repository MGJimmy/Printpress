namespace Printpress.Application;

public interface ICashByDocumentReportService
{
    Task<CashByDocumentReportDto> GetReportAsync(Guid? cashAccountId, DateTime? dateFrom, DateTime? dateTo);
}
