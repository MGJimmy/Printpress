using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using MediatR;
using UserService.Consts;
using UserService.Entities;

namespace UserService
{

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IIdmProvider<User> _idmProvider;

        public LoginCommandHandler(ITokenProvider tokenProvider, IIdmProvider<User> idmProvider)
        {
            _tokenProvider = tokenProvider;
            _idmProvider = idmProvider;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validate user credentials using IdmProvider
            var user = await _idmProvider.FindByNameAsync(request.Username);
            var loginStatus = await _idmProvider.LoginUserAsync(request.Username, request.Password);
            if (user != null && loginStatus == LoginStatus.Succeeded)
            {
                var userRoles = await _idmProvider.GetUserRoles(user);
                string rolesJson = JsonSerializer.Serialize(userRoles);

                // Generate confirmation token
                var claims = new List<Claim>
                {
                new Claim(AppClaimType.Email, user.Email) ,
                new Claim(AppClaimType.NameIdentifier,user.Id),
                new Claim(AppClaimType.Username, user.UserName),
                new Claim(AppClaimType.Roles,rolesJson ,JsonClaimValueTypes.Json)
                };

                var token = _tokenProvider.GenerateAccessToken(claims);

                return new LoginResponse
                {
                    Success = true,
                    Token = token
                };
            }

            return new LoginResponse
            {
                Success = false,
                Token = null,
                Message = loginStatus.ToString()
            };
        }
    }
}