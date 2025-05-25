using System.Security.Claims;
using UserService.Consts;

namespace Printpress.API;

[ApiController]
public class AppBaseController : ControllerBase
{
    protected string? UserId  => User.FindFirstValue(AppClaimType.NameIdentifier);
}

