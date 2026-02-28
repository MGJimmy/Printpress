using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class InventoryController(IInventoryItemService _inventoryItemService) : AppBaseController
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _inventoryItemService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(int id)
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
    public async Task<IActionResult> Update(int id, InventoryItemUpdateDto payload)
    {
        var result = await _inventoryItemService.UpdateAsync(id, payload, UserId);
        return Ok(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _inventoryItemService.DeleteAsync(id, UserId);
        return Ok();
    }
}
