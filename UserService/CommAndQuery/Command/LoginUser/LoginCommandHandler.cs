using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserService.Consts;
using UserService.Entities;

namespace UserService
{

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponse>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IIdmProvider<User> _idmProvider;

        public LoginCommandHandler(ITokenProvider tokenProvider, IIdmProvider<User> idmProvider)
        {
            _tokenProvider = tokenProvider;
            _idmProvider = idmProvider;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validate user credentials using IdmProvider
            var user = await _idmProvider.FindByNameAsync(request.Username);
            var loginStatus = await _idmProvider.LoginUserAsync(request.Username, request.Password);

            if (user is null || loginStatus != LoginStatus.Succeeded)
            {
                return new LoginCommandResponse
                {
                    LoginResponse = new LoginResponse
                    {
                        Success = false,
                        Token = null,
                        Message = loginStatus.ToString()
                    },
                    RefreshToken = string.Empty
                };
            }


            var userRoles = await _idmProvider.GetUserRoles(user);
            string rolesJson = JsonSerializer.Serialize(userRoles);

            // Generate confirmation token
            var claims = new List<Claim>
            {
                new Claim(AppClaimType.Email, user.Email) ,
                new Claim(AppClaimType.NameIdentifier,user.Id),
                new Claim(AppClaimType.Username, user.UserName),
                new Claim(AppClaimType.Roles, rolesJson, JsonClaimValueTypes.Json)
            };

            var token = await _tokenProvider.GenerateAccessToken(claims);

            string refreshToken = await _tokenProvider.GenerateRefreshToken(user.Id);

            var loginResponse = new LoginResponse
            {
                Success = true,
                Token = token
            };

            return new LoginCommandResponse
            {
                LoginResponse = loginResponse,
                RefreshToken = refreshToken
            };
        }


    }
}