using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Printpress.Application;
using Printpress.Infrastructure;

namespace Printpress.CompositionRoot
{
    public static class CompositionRoot
    {
        public static IServiceCollection RegisterCompositionRootServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services.RegisterDbContext(configuration)
                            .RegisterServices(configuration)
                            .RegisterMappers(configuration);
        }

        public static IServiceCollection RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<ApplicationDbContext>(option =>

                option.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
             );
        }


        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IClientService, ClientService>();


            return services;
        }

        public static IServiceCollection RegisterMappers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ClientMapper>();

            return services;
        }



    }
}
