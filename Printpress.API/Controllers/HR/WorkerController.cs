using Microsoft.AspNetCore.Authorization;
using Printpress.Application;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class WorkerController(IWorkerService _service, IWorkerAttendanceService _attendanceService) : AppBaseController
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paging = new Paging(pageNumber, pageSize);
        var result = await _service.GetAllAsync(paging);
        return Ok(result);
    }

    [HttpGet("getActive")]
    public async Task<IActionResult> GetActive()
    {
        var result = await _service.GetActiveAsync();
        return Ok(result);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromQuery] DateTime? productionDateFrom,
        [FromQuery] DateTime? productionDateTo)
    {
        var result = await _service.GetDetailsAsync(id, UtcDateTime.AsUtc(productionDateFrom), UtcDateTime.AsUtc(productionDateTo));
        return Ok(result);
    }

    [HttpGet("getWorkerProduction/{workerId}")]
    public async Task<IActionResult> GetWorkerProduction(
    Guid workerId,
    [FromQuery] DateTime? productionDateFrom,
    [FromQuery] DateTime? productionDateTo,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        var paging = new Paging(pageNumber, pageSize);

        var result = await _service.GetWorkerProduction(workerId, paging, UtcDateTime.AsUtc(productionDateFrom), UtcDateTime.AsUtc(productionDateTo));
        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(WorkerCreateDto payload)
    {
        var result = await _service.CreateAsync(payload, UserId);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(WorkerUpdateDto payload)
    {
        var result = await _service.UpdateAsync(payload, UserId);
        return Ok(result);
    }

    [HttpPut("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _service.DeactivateAsync(id, UserId);
        return Ok();
    }


    [HttpPut("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
              {
        await _service.activateAsync(id, UserId);
        return Ok();
    }

    [HttpGet("{id}/attendance")]
    public async Task<IActionResult> GetAttendance(
        Guid id,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        var result = await _attendanceService.GetByWorkerAsync(
            id,
            UtcDateTime.StartOfDay(dateFrom),
            UtcDateTime.ExclusiveEnd(dateTo));
        return Ok(result);
    }

    [HttpPost("{id}/attendance/check-in")]
    public async Task<IActionResult> CheckIn(Guid id, [FromBody] WorkerAttendanceTimeDto payload)
    {
        await _attendanceService.CheckInAsync(id, payload?.At, UserId);
        return Ok();
    }

    [HttpPost("{id}/attendance/check-out")]
    public async Task<IActionResult> CheckOut(Guid id, [FromBody] WorkerAttendanceTimeDto payload)
    {
        await _attendanceService.CheckOutAsync(id, payload?.At, UserId);
        return Ok();
    }

    [HttpPost("{id}/attendance/closed")]
    public async Task<IActionResult> AddClosed(Guid id, [FromBody] WorkerAttendanceClosedCreateDto payload)
    {
        await _attendanceService.AddClosedAsync(id, payload?.CheckInAt, payload?.CheckOutAt, UserId);
        return Ok();
    }

    [HttpPost("{id}/attendance/{attendanceId}/void")]
    public async Task<IActionResult> VoidAttendance(Guid id, Guid attendanceId, [FromBody] VoidInvoiceDto payload)
    {
        await _attendanceService.VoidAsync(id, attendanceId, payload?.Reason, UserId);
        return Ok();
    }
}
