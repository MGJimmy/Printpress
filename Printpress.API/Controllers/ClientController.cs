using Microsoft.AspNetCore.Mvc;
using Printpress.Application;
namespace Printpress.API.Controllers;



[Route("api/[controller]")]
[ApiController]
public class ClientController(IClientService _IClientService) : ControllerBase
{
    [HttpGet]
    [Route("getClientById")]
    public async Task<IActionResult> GetClientById(int id)
    {
        var result = await _IClientService.GetClientById(id);
        return Ok(result);
    }
}
