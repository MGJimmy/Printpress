using Microsoft.AspNetCore.Mvc;
using Printpress.Application;
namespace Printpress.API;

[Route("api/[controller]")]
[ApiController]
public class OrderController(IOrderService _IOrderService) : ControllerBase
{
    [HttpGet]
    [Route("getOrderSummaryList")]
    public async Task<IActionResult> GetOrderSummaryList(int pageNumber, int pageSize)
    {
        var result = await _IOrderService.GetOrderSummaryListAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpPost]
    [Route("insert")]
    public async Task<IActionResult> Insert(OrderDto order)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.ValidationState); // ValidationState.
        }

        await _IOrderService.InsertOrder(order);
        return Ok();
    }
}
