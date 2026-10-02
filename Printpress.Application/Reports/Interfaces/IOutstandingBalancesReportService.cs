namespace Printpress.Application;

public interface IOutstandingBalancesReportService
{
    Task<OutstandingBalancesReportDto> GetReportAsync();
}
