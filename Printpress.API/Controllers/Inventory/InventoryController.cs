using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class InventoryController(IInventoryItemService _inventoryItemService) : AppBaseController
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _inventoryItemService.GetAllAsync(new Paging(pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("getByCategory/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var result = await _inventoryItemService.GetByCategoryAsync(categoryId);
        return Ok(result);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _inventoryItemService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(InventoryItemAddDto payload)
    {
        var result = await _inventoryItemService.AddAsync(payload, UserId);
        return Ok(result);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(Guid id, InventoryItemUpdateDto payload)
    {
        var result = await _inventoryItemService.UpdateAsync(id, payload, UserId);
        return Ok(result);
    }


    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _inventoryItemService.DeleteAsync(id, UserId);
        return Ok();
    }
}
