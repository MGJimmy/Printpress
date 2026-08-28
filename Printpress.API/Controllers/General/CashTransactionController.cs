using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[Authorize]
public class CashTransactionController(ICashTransactionService _cashTransactionService) : AppBaseController
{
    [HttpGet("getByCashAccountId/{cashAccountId}")]
    public async Task<IActionResult> GetByCashAccountId(
        Guid cashAccountId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] string? type = null,
        [FromQuery] string? category = null)
    {
        var result = await _cashTransactionService.GetByCashAccountIdAsync(
            cashAccountId,
            new Paging(pageNumber, pageSize),
            UtcDateTime.AsUtc(dateFrom),
            UtcDateTime.AsUtc(dateTo),
            type,
            category);
        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddCashTransactionDto payload)
    {
        var result = await _cashTransactionService.AddAsync(payload, UserId);
        return Ok(result);
    }

    [HttpPost("void/{id}")]
    public async Task<IActionResult> Void(Guid id, [FromBody] VoidCashTransactionDto payload)
    {
        await _cashTransactionService.VoidAsync(id, payload, UserId);
        return Ok();
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferCashTransactionDto payload)
    {
        await _cashTransactionService.TransferAsync(payload, UserId);
        return Ok();
    }

    [HttpGet("external-orders")]
    public async Task<IActionResult> GetExternalOrders()
    {
        var result = await _cashTransactionService.GetExternalOrdersAsync();
        return Ok(result);
    }
}
