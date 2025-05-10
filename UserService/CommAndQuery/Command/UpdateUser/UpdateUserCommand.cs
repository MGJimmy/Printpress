using MediatR;
using UserService.Entities;

namespace UserService
{
    public class UpdateUserResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UpdateUserCommand : IRequest<UpdateUserResponse>
    {
        public User User { get; set; }
    }

}