using Microsoft.AspNetCore.Authorization;
using Printpress.Application;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class PayrollPeriodController(IPayrollPeriodService _service) : AppBaseController
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var paging = new Paging(pageNumber, pageSize);

        var result = await _service.GetAllAsync(paging);
        return Ok(result);
    }

    [HttpGet("getOpenPeriods")]
    public async Task<IActionResult> GetOpenPeriods()
    {
        var result = await _service.GetOpenPeriodsAsync();
        return Ok(result);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetDetailsAsync(id);
        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(PayrollPeriodCreateDto payload)
    {
        var result = await _service.CreateAsync(payload, UserId);
        return Ok(result);
    }

    [HttpPut("close/{id}")]
    public async Task<IActionResult> Close(Guid id)
    {
        await _service.CloseAsync(id, UserId);
        return Ok();
    }
}
