using System.Security.Claims;
using SecurityProvider;

namespace Printpress.Application
{
    internal sealed class AccountService : IAccountService
    {
        private readonly ITokenProvider _ITokenProvider;
        private readonly IIdentityProvider<IApplicationUser> _IIdentityProvider;

        public AccountService(ITokenProvider tokenProvider, IIdentityProvider<IApplicationUser> identityProvider)
        {
            _ITokenProvider = tokenProvider;
            _IIdentityProvider = identityProvider;
        }

        public async Task<string> Login(string username, string password)
        {

            var user = await _IIdentityProvider.FindUserByEmailAsync(username);

            if (user == null)
            {
                ValidationExeption.FireValidationException("User Not Found");
            }

            var loginStatus = await _IIdentityProvider.LoginUserAsync(username, password);

            if (loginStatus == LoginStatus.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(CustomClaims.UserId, user.Id),
                    new Claim(CustomClaims.UserName, user.UserName),
                    new Claim(CustomClaims.UserEmail, user.Email)
                };

                var token = _ITokenProvider.GenerateAccessToken(claims);

                return token.Token;
            }
            else
            {
                throw new ValidationExeption("Invalid credentials");
            }

        }
    }
}
