using System.Security.Claims;
using Identity.Service;

namespace Printpress.API;

[ApiController]
public class AppBaseController : ControllerBase
{
    protected string? UserId  => User.FindFirstValue(AppClaimType.NameIdentifier);
}

