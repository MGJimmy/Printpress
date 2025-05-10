using MediatR;
using UserService;
using UserService.Entities;

public class UpdateUserCommanddHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    private readonly IIdmProvider<User> _idmProvider;

    public UpdateUserCommanddHandler(IIdmProvider<User> idmProvider)
    {
        _idmProvider = idmProvider;
    }

    public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var (success, errorMessage) = await _idmProvider.UpdateUserAsync(request.User);
        return new UpdateUserResponse
        {
            Success = success,
            ErrorMessage = errorMessage
        };
    }
}