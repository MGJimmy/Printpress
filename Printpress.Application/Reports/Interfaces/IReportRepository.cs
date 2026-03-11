namespace Printpress.Application;

public interface IReportRepository
{
    Task<InventoryItemReportData?> GetInventoryItemDataAsync(Guid inventoryItemId);
    Task<int> GetInventoryUnitsInAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<int> GetInventoryUnitsOutAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<List<OrderItemUsageProjection>> GetOrderItemsUsageAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<List<InventoryCategoryFilterDto>> GetInventoryCategoriesForReportAsync();
    Task<List<InventoryItemFilterDto>> GetInventoryItemsForReportAsync(int categoryId);
}
