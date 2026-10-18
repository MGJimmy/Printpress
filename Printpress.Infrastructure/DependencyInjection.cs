using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Printpress.Application;
using Printpress.Domain;

namespace Printpress.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IGuidGenerator, GuidGenerator>();
        services.AddHttpContextAccessor();
        services.AddSingleton<ILocalizationService, LocalizationService>();

        services.AddScoped(
            sp => sp.GetRequiredService<IUnitOfWork>().InventoryItemRepository);


        #region  Repositories

        services.AddScoped<IWorkerSalaryTransactionRepository, WorkerSalaryTransactionRepository>();

        #endregion


        services.AddDbContext<ApplicationDbContext>(option =>
        {
            option.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }

}
