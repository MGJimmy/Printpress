using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Service;

public class UpdatePasswordAnonymousCommandHandler
    : IRequestHandler<UpdatePasswordAnonymousCommand, ChangePasswordResponse>
{
    private readonly UserManager<User> _userManager;

    public UpdatePasswordAnonymousCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ChangePasswordResponse> Handle(
        UpdatePasswordAnonymousCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.NewPassword))
            return new ChangePasswordResponse { Success = false, ErrorMessage = "اسم المستخدم وكلمة المرور مطلوبان" };

        var user = await _userManager.FindByNameAsync(request.Username.Trim());
        if (user is null)
            return new ChangePasswordResponse { Success = false, ErrorMessage = "المستخدم غير موجود" };

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (result.Succeeded)
            return new ChangePasswordResponse { Success = true };

        return new ChangePasswordResponse
        {
            Success = false,
            ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description))
        };
    }
}
