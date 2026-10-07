using Microsoft.AspNetCore.Authorization;
using Printpress.Application;
using Printpress.Domain;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class InventoryConversionController(IInventoryConversionService _service) : AppBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InventoryConversionCreateDto payload)
    {
        await _service.CreateAsync(payload, UserId);
        return Ok();
    }

    [HttpPost("complete/{id}")]
    public async Task<IActionResult> Complete(Guid id)
    {
        await _service.CompleteAsync(id, UserId);
        return Ok();
    }

    [HttpPost("void/{id}")]
    public async Task<IActionResult> Void(Guid id, [FromBody] VoidInvoiceDto payload)
    {
        await _service.VoidAsync(id, payload?.Reason, UserId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] Guid? itemId,
        [FromQuery] InventoryConversionStatus? status,
        [FromQuery] bool? isVoided,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _service.GetAllAsync(categoryId, itemId, status, isVoided, from, toExclusive);
        return Ok(result);
    }
}
