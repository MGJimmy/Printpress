using System.Security.Claims;
using MediatR;
using UserService;

namespace Printpress.Application.Commands
{
    public class RegisterCommand : IRequest<RegistrationResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
    public class RegisterHandler : IRequestHandler<RegisterCommand, RegistrationResponse>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IdmProvider<User> _idmProvider;

        public RegisterHandler(ITokenProvider tokenProvider, IdmProvider<User> idmProvider)
        {
            _tokenProvider = tokenProvider;
            _idmProvider = idmProvider;
        }

        public async Task<RegistrationResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if the user already exists
            var existingUser = await _idmProvider.FindByNameAsync(request.Email);
            if (existingUser != null)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Message = "User already exists",
                    ConfirmationToken = null
                };
            }

            // Register the user
            var user = new User { UserName = request.Username, Email = request.Email };
            var registrationResult = await _idmProvider.RegisterUserAsync(user, request.Password);
            if (!registrationResult)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Message = "User registration failed",
                    ConfirmationToken = null
                };
            }

            // Generate confirmation token
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, request.Email) };
            var confirmationToken = _tokenProvider.GenerateAccessToken(claims);

            return new RegistrationResponse
            {
                Success = true,
                Message = "User registered successfully",
                ConfirmationToken = confirmationToken
            };
        }
    }
}