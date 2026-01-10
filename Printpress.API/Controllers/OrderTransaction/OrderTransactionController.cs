using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;


[Route("api/[controller]")]
[AllowAnonymous]
public class OrderTransactionController(IOrderTransactionService _orderTransactionService) : AppBaseController
{
    [HttpGet]
    [Route("getByPage")]
    public async Task<IActionResult> GetByPage(int orderId, int pageNumber, int pageSize)
    {
        PagedList<OrderTransactionDto> result = await _orderTransactionService.GetByPage(orderId, pageNumber, pageSize);
        return Ok(result);
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> Add(OrderTransactionAddDto payload)
    {
        var result = await _orderTransactionService.AddAsync(payload, UserId);
        return Ok(result);
    }
}
