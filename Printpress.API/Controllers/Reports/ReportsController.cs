using Microsoft.AspNetCore.Authorization;
using Printpress.Application;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class ReportsController(
    IOrderInventoryItemsReportService _reportService,
    IInventoryServicesUsageReportService _inventoryServicesService) : AppBaseController
{
    [HttpGet("order-inventory-items")]
    public async Task<IActionResult> GetOrderInventoryItemsReport(
        [FromQuery] Guid inventoryItemId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var result = await _reportService.GetReportAsync(inventoryItemId, dateFrom, dateTo);
        return Ok(result);
    }


    // ── Report 2: Inventory & Services Usage ────────────────────────────────

    [HttpGet("inventory-services-usage")]
    public async Task<IActionResult> GetInventoryServicesUsage(
        [FromQuery] int inventoryItemCategoryId,
        [FromQuery] Guid serviceCategoryId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var result = await _inventoryServicesService.GetReportAsync(
            inventoryItemCategoryId, serviceCategoryId, dateFrom, dateTo);
        return Ok(result);
    }

    [HttpGet("filter/service-categories")]
    public async Task<IActionResult> GetServiceCategories()
    {
        var result = await _inventoryServicesService.GetServiceCategoriesAsync();
        return Ok(result);
    }
}
