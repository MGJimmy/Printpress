using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Printpress.Domain;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class InventoryTransactionController(IInventoryTransactionService _service) : AppBaseController
{
    [HttpGet("getByItemId/{itemId}")]
    public async Task<IActionResult> GetByItemId(Guid itemId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
        [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null, [FromQuery] string transactionType = null)
    {
        var result = await _service.GetByItemIdAsync(itemId, new Paging(pageNumber, pageSize), UtcDateTime.AsUtc(dateFrom), UtcDateTime.AsUtc(dateTo), transactionType);
        return Ok(result);
    }

    [HttpPost("stock-out")]
    public async Task<IActionResult> StockOut([FromBody] StockOutCreateDto payload)
    {
        await _service.StockOutAsync(payload, UserId);
        return Ok();
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] Guid? itemId,
        [FromQuery] Guid? workerId,
        [FromQuery] InventoryTransactionType? type,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _service.GetAllAsync(categoryId, itemId, workerId, type, from, toExclusive);
        return Ok(result);
    }

    [HttpGet("getByWorkerId/{workerId}")]
    public async Task<IActionResult> GetByWorkerId(
        Guid workerId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? inventoryItemCategoryId = null,
        [FromQuery] Guid? inventoryItemId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var paging = new Paging(pageNumber, pageSize);
        var result = await _service.GetByWorkerIdAsync(workerId, paging, inventoryItemCategoryId, inventoryItemId, UtcDateTime.AsUtc(dateFrom), UtcDateTime.AsUtc(dateTo));
        return Ok(result);
    }
}
