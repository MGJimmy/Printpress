﻿using Microsoft.AspNetCore.Mvc;
using Printpress.Application;
namespace Printpress.API;


[Route("api/[controller]")]
[ApiController]
public class ServiceController(IServiceService _serviceService) : ControllerBase
{
    [HttpGet]
    [Route("getByPage")]
    public async Task<IActionResult> GetByPage()
    {
        List<ServiceDto> result = await _serviceService.GetAll();
        return Ok(result);
    }

    [HttpGet]
    [Route("getById")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _serviceService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> Add(ServiceUpsertDto payload)
    {
        var result = await _serviceService.AddAsync(payload);
        return Ok(result);
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<IActionResult> Update(int id, ServiceUpsertDto payload)
    {
        var result = await _serviceService.UpdateAsync(id, payload);
        return Ok(result);
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _serviceService.DeleteAsync(id);
        return Ok();
    }
}
