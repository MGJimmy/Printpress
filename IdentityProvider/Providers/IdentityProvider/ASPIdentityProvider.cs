using Microsoft.AspNetCore.Identity;

namespace SecurityProvider
{
    internal sealed class ASPIdentityProvider : IIdentityProvider<IApplicationUser>
    {
        private readonly UserManager<AplicationUser> _userManager;
        public ASPIdentityProvider(UserManager<AplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IApplicationUser> FindUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<LoginStatus> LoginUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return LoginStatus.WrongEmail;
            }

            return LoginStatus.Succeeded;
        }
        public async Task<bool> RegisterUserAsync(IApplicationUser user, string password)
        {
            var application = user as AplicationUser;
            var result = await _userManager.CreateAsync(application, password);
            return result.Succeeded;
        }
        public async Task<List<string>> GetUserRoles(IApplicationUser user)
        {
            var application = user as AplicationUser;
            var roles = await _userManager.GetRolesAsync(application);
            return roles.ToList();
        }
    }
}