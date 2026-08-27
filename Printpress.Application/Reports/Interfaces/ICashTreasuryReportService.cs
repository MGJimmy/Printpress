namespace Printpress.Application;

public interface ICashTreasuryReportService
{
    Task<CashTreasuryReportDto> GetReportAsync(DateTime? dateFrom, DateTime? dateTo);
}
