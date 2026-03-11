namespace Printpress.Application;

public interface IInventoryServicesUsageReportService
{
    Task<InventoryServicesUsageReportDto> GetReportAsync(
        int inventoryItemCategoryId, Guid serviceCategoryId, DateTime? dateFrom, DateTime? dateTo);

    Task<List<ServiceCategoryFilterDto>> GetServiceCategoriesAsync();
}
