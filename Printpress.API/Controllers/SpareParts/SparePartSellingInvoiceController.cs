using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class SparePartSellingInvoiceController(ISparePartSellingInvoiceService _service) : AppBaseController
{
    [HttpPost("add")]
    public async Task<IActionResult> Add(SparePartSellingInvoiceCreateDto payload)
    {
        var result = await _service.CreateAsync(payload, UserId);
        return Ok(result);
    }
}
