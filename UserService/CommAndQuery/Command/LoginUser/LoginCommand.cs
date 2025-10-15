using MediatR;

namespace UserService
{
    public class LoginCommandResponse : TokenResponse
    {
    }


    public class LoginCommand : IRequest<LoginCommandResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }



}