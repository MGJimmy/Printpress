using Microsoft.AspNetCore.Authorization;
using Printpress.Application;
using Printpress.Domain;

namespace Printpress.API;

[Route("api/[controller]")]
[Authorize]
public class LoanController(ILoanService loans) : AppBaseController
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? lenderId = null,
        [FromQuery] bool? hasRemaining = null,
        [FromQuery] bool? isVoided = null,
        [FromQuery] DateOnly? dateFrom = null,
        [FromQuery] DateOnly? dateTo = null)
    {
        var result = await loans.GetAllAsync(
            lenderId,
            hasRemaining,
            isVoided,
            UtcDateTime.StartOfDay(dateFrom),
            UtcDateTime.ExclusiveEnd(dateTo),
            pageNumber,
            pageSize);
        return Ok(result);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await loans.GetByIdAsync(id));
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(LoanCreateDto payload)
    {
        return Ok(await loans.AddAsync(payload, UserId));
    }

    [HttpPost("pay/{id}")]
    public async Task<IActionResult> Pay(Guid id, [FromBody] LoanPayDto payload)
    {
        await loans.PayAsync(id, payload ?? new LoanPayDto(), UserId);
        return Ok();
    }

    [HttpPost("void/{id}")]
    public async Task<IActionResult> Void(Guid id, [FromBody] VoidInvoiceDto payload)
    {
        await loans.VoidAsync(id, payload?.Reason, UserId);
        return Ok();
    }

    [HttpPost("voidPayment/{id}")]
    public async Task<IActionResult> VoidPayment(Guid id, [FromBody] LoanVoidPaymentDto payload)
    {
        await loans.VoidPaymentAsync(id, payload ?? new LoanVoidPaymentDto(), UserId);
        return Ok();
    }
}
