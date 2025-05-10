using MediatR;
using Microsoft.AspNetCore.Authorization;
using UserService;
using UserService.Consts;

namespace Printpress.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SkipResponseWrapperFilter]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("create-user")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("assign-user-role")]
        [AllowAnonymous]
        public async Task<IActionResult> AssignRolesToUser([FromBody] AssignUserRolesCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }


        [HttpGet("get-user")]
        [Role(RoleName.Admin)]
        public async Task<IActionResult> GetUser([FromQuery] string username)
        {
            var query = new GetUserQuery(username);
            return Ok(await _mediator.Send(query));
        }

        [HttpGet("get-all-users")]
        [Role(RoleName.Admin)]
        public async Task<IActionResult> GetAllUsers()
        {
            var query = new GetAlluserQuery();
            return Ok(await _mediator.Send(query));
        }

        [HttpGet("get-user-roles")]
        [Role(RoleName.Admin)]
        public async Task<IActionResult> GetUserRoles([FromQuery] string username)
        {
            var query = new GetUserRolesQuery { Username = username };
            return Ok(await _mediator.Send(query));
        }
    }
}
