using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class SparePartController(ISparePartItemService _service) : AppBaseController
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(new Paging(pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(SparePartItemAddDto payload)
    {
        var result = await _service.AddAsync(payload, UserId);
        return Ok(result);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(Guid id, SparePartItemUpdateDto payload)
    {
        var result = await _service.UpdateAsync(id, payload, UserId);
        return Ok(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id, UserId);
        return Ok();
    }
}
