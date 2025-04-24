using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace SecurityProvider;
internal sealed class TokenProvider : ITokenProvider
{
    private readonly JwtOption _JwtConfigration;

    public TokenProvider(IOptions<JwtOption> jwtOptions)
    {
        _JwtConfigration = jwtOptions.Value;
    }

    public AccessToken GenerateAccessToken(IEnumerable<Claim> claims)
    {

        var key = Encoding.UTF8.GetBytes(_JwtConfigration.SecretKey);

        var symmetricSecurityKey = new SymmetricSecurityKey(key);

        var issuedAt = DateTime.UtcNow;

        var expires = DateTime.UtcNow.AddMinutes(_JwtConfigration.ExpiryMinutes);

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var JwtSecurityToken = new JwtSecurityToken(issuer: _JwtConfigration.Issuer,
                                                    audience: _JwtConfigration.Audience,
                                                    expires: expires,
                                                    notBefore: issuedAt,
                                                    claims: claims,
                                                    signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);

        return new AccessToken { Token = token, ExpirationTime = expires };
    }
}
