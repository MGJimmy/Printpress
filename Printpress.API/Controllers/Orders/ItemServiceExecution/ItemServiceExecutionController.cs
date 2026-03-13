using Microsoft.AspNetCore.Authorization;
using Printpress.Application;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class ItemServiceExecutionController(IItemServiceExecutionService _service) : AppBaseController
{
    [HttpGet("group-items/{groupId}")]
    public async Task<IActionResult> GetGroupItems(Guid groupId)
    {
        var result = await _service.GetGroupItemsWithProgressAsync(groupId);
        return Ok(result);
    }

    [HttpGet("item-summary/{itemId}")]
    public async Task<IActionResult> GetItemSummary(Guid itemId)
    {
        var result = await _service.GetItemExecutionSummaryAsync(itemId);
        return Ok(result);
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute([FromBody] ExecuteServiceRequestDto payload)
    {
        await _service.ExecuteAsync(payload, UserId);
        return Ok();
    }
}
