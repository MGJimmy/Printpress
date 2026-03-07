using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Printpress.Domain;

public static class DependencyInjection
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();

        return services;
    }

}
