using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace UserService;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    }

    public IEnumerable<string> GetCurrentUserRoles()
    {
        var roles = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);

        if (roles != null)
        {

           return JsonSerializer.Deserialize<IEnumerable<string>>(roles?.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        }
        return Enumerable.Empty<string>();
    }
}