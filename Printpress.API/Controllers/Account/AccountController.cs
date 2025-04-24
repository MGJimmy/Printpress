using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Printpress.Application;

namespace Printpress.API.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAccountService _IAccountService) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Login(string username, string password)
        {
            var result = _IAccountService.Login(username,password);
            return Ok(result);
        }
    }
}
