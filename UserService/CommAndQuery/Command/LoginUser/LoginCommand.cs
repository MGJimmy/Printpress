using MediatR;

namespace UserService
{
    public class LoginCommandResponse
    {
        public LoginResponse LoginResponse { get; set; }
        public string RefreshToken { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AccessToken Token { get; set; } // Updated to use AccessToken
    }
    public class LoginCommand : IRequest<LoginCommandResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AccessToken
    {
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; } 
    }

}