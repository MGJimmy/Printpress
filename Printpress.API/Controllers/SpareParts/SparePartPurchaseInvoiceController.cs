using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class SparePartPurchaseInvoiceController(ISparePartPurchaseInvoiceService _service) : AppBaseController
{
    [HttpPost("add")]
    public async Task<IActionResult> Add(SparePartPurchaseInvoiceCreateDto payload)
    {
        await _service.CreateAsync(payload, UserId);
        return Ok();
    }
}
