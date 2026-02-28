using MediatR;

namespace Identity.Service
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