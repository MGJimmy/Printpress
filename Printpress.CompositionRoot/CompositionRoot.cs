using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Printpress.Application;
using Printpress.Application.Mappers;
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
            {
                option.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                option.LogTo((log) => Console.Write(log), LogLevel.Information);
            });
        }


        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IOrderTransactionService, OrderTransactionService>();
            services.AddScoped<IOrderAggregateService, OrderAggregateService>();
            services.AddScoped<IServiceService, ServiceService>();



            return services;
        }

        public static IServiceCollection RegisterMappers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ClientMapper>();
            services.AddScoped<OrderTransactionMapper>();
            services.AddScoped<OrderMapper>();
            services.AddScoped<OrderGroupMapper>();
            services.AddScoped<GroupServiceMapper>();
            services.AddScoped<OrderServiceMapper>();
            services.AddScoped<ItemMapper>();
            services.AddScoped<ServiceMapper>();
            services.AddScoped<ItemDetailsMapper>();


            return services;
        }



    }
}
