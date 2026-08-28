namespace Printpress.Application;

public interface IZeroOrdersReportService
{
    Task<ZeroOrdersReportDto> GetReportAsync(DateTime? dateFrom, DateTime? dateToExclusive);
}
