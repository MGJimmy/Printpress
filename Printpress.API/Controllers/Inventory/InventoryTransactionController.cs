using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class InventoryTransactionController(IInventoryTransactionService _service) : AppBaseController
{
    [HttpGet("getByItemId/{itemId}")]
    public async Task<IActionResult> GetByItemId(Guid itemId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetByItemIdAsync(itemId, new Paging(pageNumber, pageSize));
        return Ok(result);
    }
}
