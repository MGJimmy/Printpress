using System.Security.Claims;

namespace UserService
{
    public interface ITokenProvider
    {
        AccessToken GenerateAccessToken(IEnumerable<Claim> claims);
    }
}
