using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Printpress.Domain;

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
        services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
        services.AddScoped<IOrderGroupService, GroupService>();

        //Mappers
        services.AddScoped<ClientMapper>();
        services.AddScoped<OrderTransactionMapper>();
        services.AddScoped<OrderMapper>();
        services.AddScoped<OrderGroupMapper>();
        services.AddScoped<GroupServiceMapper>();
        services.AddScoped<OrderServiceMapper>();
        services.AddScoped<OrderSellingItemMapper>();
        services.AddScoped<ItemMapper>();
        services.AddScoped<ServiceMapper>();
        services.AddScoped<ItemDetailsMapper>();



        //reports
        services.AddScoped<IReportFactory, InvoiceReportFactory>();
        #endregion


        #region Inventory
        services.AddScoped<IInventoryItemService, InventoryItemService>();
        services.AddScoped<IValidator<InventoryItemAddDto>, InventoryItemAddDtoValidator>();
        services.AddScoped<IValidator<InventoryItemUpdateDto>, InventoryItemUpdateDtoValidator>();
        services.AddAutoMapper(typeof(InventoryItemProfile));

        services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();
        services.AddScoped<IValidator<PurchaseInvoiceCreateDto>, PurchaseInvoiceCreateDtoValidator>();
        services.AddAutoMapper(typeof(PurchaseInvoiceProfile));

        services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
        services.AddScoped<IValidator<StockOutCreateDto>, StockOutCreateDtoValidator>();
        services.AddAutoMapper(typeof(InventoryTransactionProfile));
        #endregion


        #region SpareParts
        services.AddScoped<ISparePartItemService, SparePartItemService>();
        services.AddScoped<IValidator<SparePartItemAddDto>, SparePartItemAddDtoValidator>();
        services.AddScoped<IValidator<SparePartItemUpdateDto>, SparePartItemUpdateDtoValidator>();
        services.AddAutoMapper(typeof(SparePartItemProfile));

        services.AddScoped<ISparePartPurchaseInvoiceService, SparePartPurchaseInvoiceService>();

        services.AddScoped<ISparePartSellingInvoiceService, SparePartSellingInvoiceService>();
        services.AddScoped<IValidator<SparePartSellingInvoiceCreateDto>, SparePartSellingInvoiceCreateDtoValidator>();
        #endregion


        #region HR
        services.AddScoped<IPayrollPeriodService, PayrollPeriodService>();
        services.AddScoped<IValidator<PayrollPeriodCreateDto>, PayrollPeriodCreateDtoValidator>();
        #endregion

        #region Reports
        services.AddScoped<IOrderInventoryItemsReportService, OrderInventoryItemsReportService>();
        services.AddScoped<IInventoryServicesUsageReportService, InventoryServicesUsageReportService>();
        #endregion


        services.RegisterDomainServices(configuration);
        
        return services;

    }



}
