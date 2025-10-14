using System.Security.Claims;
using UserService.Entities;

namespace UserService
{
    public interface ITokenProvider
    {
        Task<AccessToken> GenerateAccessToken(IEnumerable<Claim> claims);
        Task<string> GenerateRefreshToken(string userId);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeRefreshTokenAsync(string token);
        Task RevokeAllUserRefreshTokensAsync(string userId);
    }
}
