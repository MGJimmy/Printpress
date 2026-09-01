using Identity.Service;

namespace Printpress.API;

public sealed class UserDisplayNameService(IIdmProvider<User> users) : IUserDisplayNameService
{
    public async Task<string> GetDisplayNameAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return "—";

        var user = await users.FindByIdAsync(userId);
        if (user is null)
            return userId;

        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        if (!string.IsNullOrWhiteSpace(fullName))
            return fullName;

        return string.IsNullOrWhiteSpace(user.UserName) ? userId : user.UserName;
    }
}
