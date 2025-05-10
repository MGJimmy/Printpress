using MediatR;
using UserService.Entities;

namespace UserService
{
    public class GetUserQuery : IRequest<User>
    {
        public string Username { get; set; }

        public GetUserQuery(string username)
        {
            Username = username;
        }
    }
}
