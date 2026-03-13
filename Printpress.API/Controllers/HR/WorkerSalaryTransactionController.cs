using Microsoft.AspNetCore.Authorization;
using Printpress.Application;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class WorkerSalaryTransactionController(IWorkerSalaryTransactionService _service) : AppBaseController
{
    [HttpPost("add")]
    public async Task<IActionResult> Add(AddSalaryTransactionDto payload)
    {
        var result = await _service.AddAsync(payload, UserId);
        return Ok(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id, UserId);
        return Ok();
    }
}
