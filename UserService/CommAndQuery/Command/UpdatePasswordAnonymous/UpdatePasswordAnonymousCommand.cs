using MediatR;

namespace Identity.Service;

public class UpdatePasswordAnonymousCommand : IRequest<ChangePasswordResponse>
{
    public string Username { get; set; }
    public string NewPassword { get; set; }
}
