namespace Printpress.Application;

public interface IReportRepository
{
    Task<InventoryItemReportData?> GetInventoryItemDataAsync(Guid inventoryItemId);
    Task<int> GetInventoryCartonsInAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<int> GetInventorycartonsOutAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<List<OrderItemUsageProjection>> GetOrderItemsUsageAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<List<InventoryCategoryFilterDto>> GetInventoryCategoriesForReportAsync();
    Task<List<InventoryCategoryFilterDto>> GetInventoryCategoriesAllAsync();
    Task<List<InventoryItemFilterDto>> GetInventoryItemsForReportAsync(int categoryId);

    // Report 2: Inventory & Services Usage
    Task<List<InventoryItemStockProjection>> GetInventoryItemsStockByCategoryAsync(int categoryId, DateTime? dateFrom, DateTime? dateTo);
    Task<List<ServiceCategoryFilterDto>> GetAllServiceCategoriesAsync();
    Task<List<ServiceBasicInfo>> GetServicesByCategoryIdAsync(Guid serviceCategoryId);
    Task<Dictionary<Guid, int>> GetOrderCountsByServiceAsync(List<Guid> serviceIds, DateTime? dateFrom, DateTime? dateTo);
    Task<List<ServiceItemRaw>> GetServiceItemRawDataAsync(List<Guid> serviceIds, DateTime? dateFrom, DateTime? dateTo);
}
