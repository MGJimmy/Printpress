using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;



using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Entities;
using UserService.Presistance;


namespace UserService
{
    internal sealed class TokenService : ITokenProvider
    {
        private readonly JwtOption _jwtConfigration;
        private readonly UserDbContext _context;

        public TokenService(IOptions<JwtOption> jwtOptions, UserDbContext context)
        {
            _jwtConfigration = jwtOptions.Value;
            _context = context;
        }

        public Task<AccessToken> GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = Encoding.UTF8.GetBytes(_jwtConfigration.SecretKey);

            var symmetricSecurityKey = new SymmetricSecurityKey(key);

            var issuedAt = DateTime.UtcNow;

            var expires = DateTime.UtcNow.AddMinutes(_jwtConfigration.AccessTokenExpirationMinutes);

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(issuer: _jwtConfigration.Issuer,
                                                        audience: _jwtConfigration.Audience,
                                                        expires: expires,
                                                        notBefore: issuedAt,
                                                        claims: claims,
                                                        signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var accessToken = new AccessToken 
            { 
                Token = token, 
                ExpirationTime = expires
            };

            return Task.FromResult(accessToken);
        }


        public async Task<string> GenerateRefreshToken(string userId)
        {
            await RevokeAllUserRefreshTokensAsync(userId);

            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            string refreshToken = Convert.ToBase64String(randomNumber);

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtConfigration.RefreshTokenExpirationDays),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await SaveRefreshTokenAsync(refreshTokenEntity);

            return refreshToken;
        }



        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);
        }


        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }


        public async Task RevokeAllUserRefreshTokensAsync(string userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
        }

    }

}
