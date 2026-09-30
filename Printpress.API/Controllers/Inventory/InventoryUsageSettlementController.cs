using Microsoft.AspNetCore.Authorization;
using Printpress.Application;
using Printpress.Domain;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class InventoryUsageSettlementController(IInventoryUsageSettlementService _service) : AppBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InventoryUsageSettlementCreateDto payload)
    {
        await _service.CreateAsync(payload, UserId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] Guid? itemId,
        [FromQuery] InventoryUsageSettlementType? type,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _service.GetAllAsync(categoryId, itemId, type, from, toExclusive);
        return Ok(result);
    }
}
