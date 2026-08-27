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
            dateFrom,
            dateTo,
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

    [HttpGet("external-orders")]
    public async Task<IActionResult> GetExternalOrders()
    {
        var result = await _cashTransactionService.GetExternalOrdersAsync();
        return Ok(result);
    }
}
