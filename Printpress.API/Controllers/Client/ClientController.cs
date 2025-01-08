using Microsoft.AspNetCore.Mvc;
using Printpress.Application;
namespace Printpress.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ClientController(IClientService _IClientService) : ControllerBase
{
    [HttpGet]
    [Route("getById")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _IClientService.GetClientById(id);
        return Ok(result);  
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> Add(ClientUpsertDto payload)
    {
        var result = await _IClientService.AddAsync(payload);
        return Ok(result);
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<IActionResult> Update(int id, ClientUpsertDto payload)
    {
        var result = await _IClientService.UpdateAsync(id, payload);
        return Ok(result);
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _IClientService.DeleteAsync(id);
        return Ok();
    }
}
