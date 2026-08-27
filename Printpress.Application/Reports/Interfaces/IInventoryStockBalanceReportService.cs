namespace Printpress.Application;

public interface IInventoryStockBalanceReportService
{
    Task<InventoryStockBalanceReportDto> GetReportAsync(int? categoryId, DateTime? dateFrom, DateTime? dateToExclusive);
}
