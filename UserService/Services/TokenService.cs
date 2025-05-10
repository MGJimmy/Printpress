using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace UserService
{
    internal sealed class TokenService : ITokenProvider
    {
        private readonly JwtOption _jwtConfigration;

        public TokenService(IOptions<JwtOption> jwtOptions)
        {
            _jwtConfigration = jwtOptions.Value;
        }

        public AccessToken GenerateAccessToken(IEnumerable<Claim> claims)
        {

            var key = Encoding.UTF8.GetBytes(_jwtConfigration.SecretKey);

            var symmetricSecurityKey = new SymmetricSecurityKey(key);

            var issuedAt = DateTime.UtcNow;

            var expires = DateTime.UtcNow.AddMinutes(_jwtConfigration.ExpiryMinutes);

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(issuer: _jwtConfigration.Issuer,
                                                        audience: _jwtConfigration.Audience,
                                                        expires: expires,
                                                        notBefore: issuedAt,
                                                        claims: claims,
                                                        signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return new AccessToken { Token = token, ExpirationTime = expires };
        }
    }

}
