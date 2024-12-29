using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Printpress.Application;

namespace Printpress.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _IClientService;

        public ClientController(IClientService clientService)
        {
            _IClientService = clientService;
        }


        [HttpGet]
        [Route("getClientById")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var result =await _IClientService.GetClientById(id);
            return Ok(result);
        }
    }
}
