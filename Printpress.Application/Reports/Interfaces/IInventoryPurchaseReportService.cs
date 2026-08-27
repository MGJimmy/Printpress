namespace Printpress.Application;

public interface IInventoryPurchaseReportService
{
    Task<InventoryPurchaseReportDto> GetReportAsync(
        int? categoryId, Guid? inventoryItemId, DateTime? dateFrom, DateTime? dateToExclusive);
}
