using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class CashAccountController(ICashAccountService _cashAccountService) : AppBaseController
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _cashAccountService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _cashAccountService.GetByIdAsync(id);
        return Ok(result);
    }
}
