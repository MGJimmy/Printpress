using System.Security.Claims;
using Printpress.Domain.Entities;
using SecurityProvider;

namespace Printpress.Application
{
    internal sealed class AccountService : IAccountService
    {
        private readonly ITokenProvider _ITokenProvider;
        private readonly IdmProvider<ApplicationUser> _IdmProvider;

        public AccountService(ITokenProvider tokenProvider, IdmProvider<ApplicationUser> identityProvider)
        {
            _ITokenProvider = tokenProvider;
            _IdmProvider = identityProvider;
        }

        public async Task<string> Login(string username, string password)
        {

            var user = await _IdmProvider.FindUserByEmailAsync(username);

            if (user == null)
            {
                ValidationExeption.FireValidationException("User Not Found");
            }

            var loginStatus = await _IdmProvider.LoginUserAsync(username, password);

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
