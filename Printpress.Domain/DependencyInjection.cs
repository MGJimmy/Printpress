using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Printpress.Domain;

public static class DependencyInjection
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IInventoryTransactionDomainService, InventoryTransactionDomainService>();
        services.AddScoped<IWorkerTransactionCalculator, WorkerTransactionCalculator>();

        services.AddScoped<CashAccountDomainService>();

        return services;
    }

}
