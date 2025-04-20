using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Printpress.API.Middlewares;
using Printpress.Application;
using Swashbuckle.AspNetCore.SwaggerGen;
using Printpress.Infrastructure;

namespace Printpress.API;

public static class IServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.RegisterApplicationService(configuration);
        services.RegisterInfrastructureServices(configuration);
        services.AddExceptionHandler<GlobalExceptionMiddleWare>();
        services.AddCros();
    }
    private static IServiceCollection AddControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ModelStateInvalidFilter>();
            options.Filters.Add<ResponseWrapperFilter>();
        });
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }
    private static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(delegate (SwaggerGenOptions swagger)
        {
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Example: 'Bearer 12345'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            } });
        });

        return services;
    }

    private static IServiceCollection AddCros(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

}
