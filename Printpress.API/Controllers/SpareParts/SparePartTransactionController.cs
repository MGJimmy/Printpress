using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class SparePartTransactionController(ISparePartTransactionService _service) : AppBaseController
{
    [HttpGet("getByItemId/{itemId}")]
    public async Task<IActionResult> GetByItemId(Guid itemId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
        [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null, [FromQuery] string transactionType = null)
    {
        var result = await _service.GetByItemIdAsync(itemId, new Paging(pageNumber, pageSize), UtcDateTime.AsUtc(dateFrom), UtcDateTime.AsUtc(dateTo), transactionType);
        return Ok(result);
    }
}
