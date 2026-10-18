using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[Authorize]
public class TodoItemController(ITodoItemService todos) : AppBaseController
{
    [HttpGet("getList")]
    public async Task<IActionResult> GetList()
    {
        return Ok(await todos.GetListAsync());
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(TodoItemAddDto payload)
    {
        return Ok(await todos.AddAsync(payload, UserId));
    }

    [HttpPost("complete/{id}")]
    public async Task<IActionResult> Complete(Guid id)
    {
        await todos.CompleteAsync(id, UserId);
        return Ok();
    }
}
