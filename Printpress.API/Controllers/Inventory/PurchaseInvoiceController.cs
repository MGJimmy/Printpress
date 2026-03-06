using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class PurchaseInvoiceController(IPurchaseInvoiceService _purchaseInvoiceService) : AppBaseController
{
    [HttpPost("add")]
    public async Task<IActionResult> Add(PurchaseInvoiceCreateDto payload)
    {
        var result = await _purchaseInvoiceService.CreateAsync(payload, UserId);
        return Ok(result);
    }
}
