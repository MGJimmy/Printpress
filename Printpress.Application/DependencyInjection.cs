using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Printpress.Application;

public static class DependencyInjection
{

    public static IServiceCollection RegisterApplicationService(this IServiceCollection services, IConfiguration configuration)
    {
        #region Orders
        //services
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IOrderTransactionService, OrderTransactionService>();
        services.AddScoped<IOrderAggregateService, OrderAggregateService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IOrderGroupService, GroupService>();

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
        #endregion


        #region Inventory
        services.AddScoped<IInventoryItemService, InventoryItemService>();
        services.AddScoped<InventoryItemMapper>();
        services.AddScoped<IValidator<InventoryItemAddDto>, InventoryItemAddDtoValidator>();
        services.AddScoped<IValidator<InventoryItemUpdateDto>, InventoryItemUpdateDtoValidator>();
        #endregion


        return services;

    }



}
