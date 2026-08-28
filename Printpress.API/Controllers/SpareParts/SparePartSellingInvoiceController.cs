using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class SparePartSellingInvoiceController(ISparePartSellingInvoiceService _service) : AppBaseController
{
    [HttpPost("add")]
    public async Task<IActionResult> Add(SparePartSellingInvoiceCreateDto payload)
    {
        var result = await _service.CreateAsync(payload, UserId);
        return Ok(result);
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? itemId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _service.GetAllAsync(itemId, from, toExclusive);
        return Ok(result);
    }
}
