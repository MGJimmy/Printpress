using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;

[Route("api/[controller]")]
[Authorize]
public class OrderController(IOrderAggregateService _IOrderService, IOrderGroupService orderGroupService) : AppBaseController
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
        return Ok(orderDto);
    }

    [HttpGet]
    [Route("getMainData/{id}")]
    public async Task<IActionResult> GetMainData(int id)
    {
        var orderDto = await _IOrderService.GetOrderMainDataAsync(id);
        return Ok(orderDto);
    }
    [HttpPost]
    [Route("insert")]
    public async Task<IActionResult> Insert(OrderUpsertDto order)
    {
        await _IOrderService.InsertOrder(order, UserId);
        return Ok();
    }
    [HttpPut]
    [Route("update/{id}")]
    public async Task<IActionResult> Update(int id, OrderUpsertDto order)
    {
        await _IOrderService.UpdateOrder(id, order, UserId);
        return Ok();
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _IOrderService.DeleteOrder(id);
        return Ok();
    }

    [HttpPost]
    [Route("DeliverOrderGroup")]
    public async Task<IActionResult> DeliverOrderGroup(DeliverGroupDto groupDeliveryDto)
    {
        var result = await orderGroupService.DeliverGroup(groupDeliveryDto);
        return Ok(result);
    }

}
