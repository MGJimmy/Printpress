namespace Printpress.Application;

public interface IReportRepository
{
    Task<InventoryItemReportData?> GetInventoryItemDataAsync(Guid inventoryItemId);
    Task<int> GetInventoryCartonsInAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<int> GetInventorycartonsOutAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<List<OrderItemUsageProjection>> GetOrderItemsUsageAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<List<InventoryCategoryFilterDto>> GetInventoryCategoriesForReportAsync();
    Task<List<InventoryItemFilterDto>> GetInventoryItemsForReportAsync(int categoryId);
}
