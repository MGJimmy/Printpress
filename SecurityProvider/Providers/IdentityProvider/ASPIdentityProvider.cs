using Microsoft.AspNetCore.Identity;

namespace SecurityProvider
{
    internal sealed class AspIdentityProvider<TUser> : IdmProvider<TUser> where TUser : class, IApplicationUser
    {
        private readonly UserManager<TUser> _userManager;
        public AspIdentityProvider(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<TUser> FindUserByEmailAsync(string email)
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
        public async Task<bool> RegisterUserAsync(TUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }
        public Task<List<string>> GetUserRoles(TUser user)
        {
            throw new NotImplementedException();
        }

    }
}