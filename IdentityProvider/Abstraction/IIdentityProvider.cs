namespace SecurityProvider;

public interface IIdentityProvider<T> where T : IApplicationUser
{
    Task<bool> RegisterUserAsync(IApplicationUser user, string password);
    Task<LoginStatus> LoginUserAsync(string email, string password);
    Task<IApplicationUser> FindUserByEmailAsync(string email);
    Task<List<string>>GetUserRoles(IApplicationUser user);
}


