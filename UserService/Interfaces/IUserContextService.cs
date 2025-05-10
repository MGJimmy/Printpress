namespace UserService;

public interface IUserContextService
{
    string GetCurrentUserId();
    string GetCurrentUserName();
    IEnumerable<string> GetCurrentUserRoles();
}