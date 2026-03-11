using Microsoft.AspNetCore.Authorization;
using Printpress.Application;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class ReportsController(IOrderInventoryItemsReportService _reportService) : AppBaseController
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

    [HttpGet("filter/inventory-categories")]
    public async Task<IActionResult> GetInventoryCategories()
    {
        var result = await _reportService.GetCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("filter/inventory-items")]
    public async Task<IActionResult> GetInventoryItemsByCategory([FromQuery] int categoryId)
    {
        var result = await _reportService.GetItemsByCategoryAsync(categoryId);
        return Ok(result);
    }
}
