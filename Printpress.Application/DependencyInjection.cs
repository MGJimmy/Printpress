using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Printpress.Application.Mappers;

namespace Printpress.Application;

public static class DependencyInjection
{

    public static IServiceCollection RegisterApplicationService(this IServiceCollection services, IConfiguration configuration)
    {
        //services
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IOrderTransactionService, OrderTransactionService>();
        services.AddScoped<IOrderAggregateService, OrderAggregateService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IOrderGroupService, OrderGroupSere>();

        //Mappers
        services.AddScoped<ClientMapper>();
        services.AddScoped<OrderTransactionMapper>();
        services.AddScoped<OrderMapper>();
        services.AddScoped<OrderGroupMapper>();
        services.AddScoped<GroupServiceMapper>();
        services.AddScoped<OrderServiceMapper>();
        services.AddScoped<ItemMapper>();
        services.AddScoped<ServiceMapper>();
        services.AddScoped<ItemDetailsMapper>();



        //reports
        services.AddScoped<IReportFactory, InvoiceReportFactory>();


        return services;

    }



}
