using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using UserService;
using UserService.Consts;

namespace Printpress.API;

[Route("api/[controller]")]
[SkipResponseWrapperFilter]
[Authorize]
public class AccountController : AppBaseController
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public AccountController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        LoginCommandResponse result = await _mediator.Send(request);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.LoginResponse);
    }

    private void SetRefreshTokenCookie(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) return;



        int refreshTokenExpirationDays = int.TryParse(_configuration.GetRequiredSection("Jwt:RefreshTokenExpirationDays").Value, out int parsedValue) ? parsedValue : 7;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Use only over HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/"
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    [HttpPost("create-user")]
    [Role(RoleName.Admin)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("assign-user-role")]
    [Role(RoleName.Admin)]
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

