namespace Printpress.Application;

public interface IOrderInventoryItemsReportService
{
    Task<OrderInventoryItemsReportDto> GetReportAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
    Task<List<InventoryCategoryFilterDto>> GetCategoriesLinkToServiceAsync();
    Task<List<InventoryCategoryFilterDto>> GetCategoriesAllAsync();
    Task<List<InventoryItemFilterDto>> GetItemsByCategoryAsync(int categoryId);
}
