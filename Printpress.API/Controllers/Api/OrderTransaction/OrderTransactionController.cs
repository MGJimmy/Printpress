using Microsoft.AspNetCore.Mvc;
using Printpress.Application;
namespace Printpress.API;


[Route("api/[controller]")]
[ApiController]
public class OrderTransactionController(IOrderTransactionService _orderTransactionService) : ControllerBase
{
    [HttpGet]
    [Route("getByPage")]
    public async Task<IActionResult> GetByPage(int pageNumber, int pageSize)
    {
        PagedList<OrderTransactionDto> result = await _orderTransactionService.GetByPage(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> Add(OrderTransactionAddDto payload)
    {
        var result = await _orderTransactionService.AddAsync(payload);
        return Ok(result);
    }
}
