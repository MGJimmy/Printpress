using MediatR;

namespace UserService
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AccessToken Token { get; set; } // Updated to use AccessToken
    }
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}