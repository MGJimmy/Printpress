using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Printpress.Infrastructure;

namespace Printpress.CompositionRoot
{
    public static class CompositionRoot
    {
        public static IServiceCollection AddCompositionRootServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                    .AddDbContext(configuration);

        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<ApplicationDbContext>(option =>

                option.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
             );
        }

    }
}
