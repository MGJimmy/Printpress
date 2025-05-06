using System.Security.Claims;
using MediatR;

namespace UserService
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IdmProvider<User> _idmProvider;

        public LoginHandler(ITokenProvider tokenProvider, IdmProvider<User> idmProvider)
        {
            _tokenProvider = tokenProvider;
            _idmProvider = idmProvider;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validate user credentials using IdmProvider
            var user = await _idmProvider.FindByNameAsync(request.Username);
            if (user != null && await _idmProvider.LoginUserAsync(request.Username, request.Password) == LoginStatus.Succeeded)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, request.Username) };
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
                Token = null
            };
        }
    }
}