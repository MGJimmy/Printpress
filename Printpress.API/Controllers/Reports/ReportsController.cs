using Microsoft.AspNetCore.Authorization;
using Printpress.Application;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class ReportsController(
    IOrderInventoryItemsReportService _reportService,
    IInventoryServicesUsageReportService _inventoryServicesService,
    ICashBookReportService _cashBookReportService,
    ICashReconcileReportService _cashReconcileReportService,
    ICashMovementSummaryReportService _cashMovementSummaryReportService,
    ICashFlowReportService _cashFlowReportService,
    ICashByDocumentReportService _cashByDocumentReportService,
    ICashTreasuryReportService _cashTreasuryReportService,
    IInventoryStockBalanceReportService _inventoryStockBalanceService,
    IInventoryPurchaseReportService _inventoryPurchaseService,
    IInventoryStockOutReportService _inventoryStockOutService,
    IInventoryMovementReportService _inventoryMovementService,
    IZeroOrdersReportService _zeroOrdersReportService) : AppBaseController
{
    [Authorize]
    [HttpGet("order-inventory-items")]
    public async Task<IActionResult> GetOrderInventoryItemsReport(
        [FromQuery] Guid inventoryItemId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _reportService.GetReportAsync(inventoryItemId, from, toExclusive);
        return Ok(result);
    }


    // ── Report 2: Inventory & Services Usage ────────────────────────────────

    [Authorize]
    [HttpGet("inventory-services-usage")]
    public async Task<IActionResult> GetInventoryServicesUsage(
        [FromQuery] int inventoryItemCategoryId,
        [FromQuery] Guid serviceCategoryId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _inventoryServicesService.GetReportAsync(
            inventoryItemCategoryId, serviceCategoryId, from, toExclusive);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("filter/service-categories")]
    public async Task<IActionResult> GetServiceCategories()
    {
        var result = await _inventoryServicesService.GetServiceCategoriesAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("inventory-stock-balance")]
    public async Task<IActionResult> GetInventoryStockBalance(
        [FromQuery] int? categoryId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _inventoryStockBalanceService.GetReportAsync(categoryId, from, toExclusive);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("inventory-purchases")]
    public async Task<IActionResult> GetInventoryPurchases(
        [FromQuery] int? categoryId,
        [FromQuery] Guid? inventoryItemId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _inventoryPurchaseService.GetReportAsync(categoryId, inventoryItemId, from, toExclusive);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("inventory-stock-out")]
    public async Task<IActionResult> GetInventoryStockOut(
        [FromQuery] int? categoryId,
        [FromQuery] Guid? inventoryItemId,
        [FromQuery] Guid? workerId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _inventoryStockOutService.GetReportAsync(
            categoryId, inventoryItemId, workerId, from, toExclusive);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("inventory-movement")]
    public async Task<IActionResult> GetInventoryMovement(
        [FromQuery] Guid inventoryItemId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _inventoryMovementService.GetReportAsync(inventoryItemId, from, toExclusive);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("cash-book")]
    public async Task<IActionResult> GetCashBook(
        [FromQuery] Guid? cashAccountId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] string? type,
        [FromQuery] string? category,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? to = UtcDateTime.StartOfDay(dateTo);
        var result = await _cashBookReportService.GetReportAsync(
            cashAccountId, from, to, type, category, search, page, pageSize);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("cash-reconcile")]
    public async Task<IActionResult> GetCashReconcile(
        [FromQuery] Guid? cashAccountId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? to = UtcDateTime.StartOfDay(dateTo);
        var result = await _cashReconcileReportService.GetReportAsync(cashAccountId, from, to);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("cash-movement-summary")]
    public async Task<IActionResult> GetCashMovementSummary(
        [FromQuery] Guid? cashAccountId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? to = UtcDateTime.StartOfDay(dateTo);
        var result = await _cashMovementSummaryReportService.GetReportAsync(cashAccountId, from, to);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("cash-flow")]
    public async Task<IActionResult> GetCashFlow(
        [FromQuery] Guid? cashAccountId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? to = UtcDateTime.StartOfDay(dateTo);
        var result = await _cashFlowReportService.GetReportAsync(cashAccountId, from, to);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("cash-by-document")]
    public async Task<IActionResult> GetCashByDocument(
        [FromQuery] Guid? cashAccountId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? to = UtcDateTime.StartOfDay(dateTo);
        var result = await _cashByDocumentReportService.GetReportAsync(cashAccountId, from, to);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("cash-treasury")]
    public async Task<IActionResult> GetCashTreasury(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? to = UtcDateTime.StartOfDay(dateTo);
        var result = await _cashTreasuryReportService.GetReportAsync(from, to);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("zero-orders")]
    public async Task<IActionResult> GetZeroOrders(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _zeroOrdersReportService.GetReportAsync(from, toExclusive);
        return Ok(result);
    }
}
