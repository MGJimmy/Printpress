namespace Printpress.Application;

public interface IUserDisplayNameService
{
    Task<string> GetDisplayNameAsync(string userId);
}
