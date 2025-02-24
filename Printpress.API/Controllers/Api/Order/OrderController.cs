using Microsoft.AspNetCore.Mvc;
using Printpress.Application;
namespace Printpress.API;

[Route("api/[controller]")]
[ApiController]
public class OrderController(IOrderAggregateService _IOrderService) : ControllerBase
{
    [HttpGet]
    [Route("getOrderSummaryList")]
    public async Task<IActionResult> GetOrderSummaryList(int pageNumber, int pageSize)
    {
        var result = await _IOrderService.GetOrderSummaryListAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet]
    [Route("getById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var orderDto = await _IOrderService.GetOrderDTOAsync(id);

        if (orderDto == null)
        {
            return NotFound(id);
        }

        return Ok(orderDto);
    }
    [HttpPost]
    [Route("insert")]
    public async Task<IActionResult> Insert(OrderUpsertDto order)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        await _IOrderService.InsertOrder(order);
        return Ok();
    }
}
