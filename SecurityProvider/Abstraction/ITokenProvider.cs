using System.Security.Claims;

namespace SecurityProvider
{
    public interface ITokenProvider
    {
        AccessToken GenerateAccessToken(IEnumerable<Claim> claims);
    }
}
