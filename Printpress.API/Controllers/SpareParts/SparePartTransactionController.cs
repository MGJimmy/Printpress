using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class SparePartTransactionController(ISparePartTransactionService _service) : AppBaseController
{
    [HttpGet("getByItemId/{itemId}")]
    public async Task<IActionResult> GetByItemId(
        Guid itemId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DateOnly? dateFrom = null,
        [FromQuery] DateOnly? dateTo = null,
        [FromQuery] string transactionType = null)
    {
        var result = await _service.GetByItemIdAsync(
            itemId,
            new Paging(pageNumber, pageSize),
            UtcDateTime.StartOfDay(dateFrom),
            UtcDateTime.ExclusiveEnd(dateTo),
            transactionType);
        return Ok(result);
    }
}
