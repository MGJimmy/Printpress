namespace SecurityProvider;

public interface IdmProvider<T> where T : IApplicationUser
{
    Task<bool> RegisterUserAsync(T user, string password);
    Task<LoginStatus> LoginUserAsync(string email, string password);
    Task<T> FindUserByEmailAsync(string email);
    Task<List<string>>GetUserRoles(T user);
}


