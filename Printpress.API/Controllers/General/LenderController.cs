using Microsoft.AspNetCore.Authorization;
using Printpress.Application;

namespace Printpress.API;

[Route("api/[controller]")]
[Authorize]
public class LenderController(ILenderService lenders) : AppBaseController
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await lenders.GetAllAsync());
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await lenders.GetByIdAsync(id));
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(LenderUpsertDto payload)
    {
        return Ok(await lenders.AddAsync(payload, UserId));
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(Guid id, LenderUpsertDto payload)
    {
        return Ok(await lenders.UpdateAsync(id, payload, UserId));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await lenders.DeleteAsync(id, UserId);
        return Ok();
    }
}
