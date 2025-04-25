using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace SecurityProvider
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddIdentityProvider<TUser>(this IServiceCollection services) where TUser : class, IApplicationUser
        {
            services.AddScoped<IdmProvider<TUser>, AspIdentityProvider<TUser>>();
            return services;
        }
        public static IServiceCollection AddRolePolicyProvider(this IServiceCollection service)
        {
            service.AddSingleton<IAuthorizationPolicyProvider, RolePolicyProvider>();
            service.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
            return service;
        }
        public static IServiceCollection AddTokenProvider(this IServiceCollection service)
        {
            service.AddSingleton<ITokenProvider, JwtTokenProvider>();
            return service;
        }
    }
}
