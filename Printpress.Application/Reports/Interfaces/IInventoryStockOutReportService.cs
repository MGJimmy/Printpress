namespace Printpress.Application;

public interface IInventoryStockOutReportService
{
    Task<InventoryStockOutReportDto> GetReportAsync(
        int? categoryId, Guid? inventoryItemId, Guid? workerId, DateTime? dateFrom, DateTime? dateToExclusive);
}
