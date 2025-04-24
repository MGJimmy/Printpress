using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace SecurityProvider
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddSecurityProvider(this IServiceCollection services, Action<SecurityOption> option = null)
        {
            var securityOptions = new SecurityOption();

            option?.Invoke(securityOptions);

            if (securityOptions.EnableSecurityProvider)
            {
                services.AddIdentityProvider();
            }

            if (securityOptions.EnableRoleBasedAuthorization)
            {
                services.AddRolePolicyProvider();
            }

            if (securityOptions.EnableTokenProvider)
            {
                services.AddTokenProvider();
            }

            return services;
        }
        private static IServiceCollection AddIdentityProvider(this IServiceCollection services)
        {
            services.AddScoped<IIdentityProvider<IApplicationUser>, ASPIdentityProvider>();
            return services;
        }
        private static IServiceCollection AddRolePolicyProvider(this IServiceCollection service)
        {
            service.AddSingleton<IAuthorizationPolicyProvider, RolePolicyProvider>();
            service.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
            return service;
        }
        private static IServiceCollection AddTokenProvider(this IServiceCollection service)
        {
            service.AddSingleton<ITokenProvider, TokenProvider>();
            return service;
        }
    }
}
