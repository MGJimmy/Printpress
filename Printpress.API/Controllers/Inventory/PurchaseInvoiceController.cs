using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class PurchaseInvoiceController(IPurchaseInvoiceService _purchaseInvoiceService) : AppBaseController
{
    [HttpPost("add")]
    public async Task<IActionResult> Add(PurchaseInvoiceCreateDto payload)
    {
        var result = await _purchaseInvoiceService.CreateAsync(payload, UserId);
        return Ok(result);
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] Guid? itemId = null,
        [FromQuery] DateOnly? dateFrom = null,
        [FromQuery] DateOnly? dateTo = null,
        [FromQuery] bool? isVoided = null,
        [FromQuery] bool? hasRemaining = null,
        [FromQuery] bool? isGoodsReceived = null)
    {
        DateTime? from = UtcDateTime.StartOfDay(dateFrom);
        DateTime? toExclusive = UtcDateTime.ExclusiveEnd(dateTo);
        var result = await _purchaseInvoiceService.GetAllAsync(
            categoryId, itemId, from, toExclusive, pageNumber, pageSize, isVoided, hasRemaining, isGoodsReceived);
        return Ok(result);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _purchaseInvoiceService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost("pay/{id}")]
    public async Task<IActionResult> Pay(Guid id, [FromBody] InvoicePayDto payload)
    {
        await _purchaseInvoiceService.PayAsync(id, payload ?? new InvoicePayDto(), UserId);
        return Ok();
    }

    [HttpPost("receive/{id}")]
    public async Task<IActionResult> Receive(Guid id)
    {
        await _purchaseInvoiceService.ReceiveGoodsAsync(id, UserId);
        return Ok();
    }

    [HttpPost("void/{id}")]
    public async Task<IActionResult> Void(Guid id, [FromBody] VoidInvoiceDto payload)
    {
        await _purchaseInvoiceService.VoidAsync(id, payload?.Reason, UserId);
        return Ok();
    }
}
