namespace Printpress.Application;

public interface ICashBookReportService
{
    Task<CashBookReportDto> GetReportAsync(
        Guid? cashAccountId,
        DateTime? dateFrom,
        DateTime? dateTo,
        string type = null,
        string category = null,
        string search = null,
        int page = 1,
        int pageSize = 10);
}
