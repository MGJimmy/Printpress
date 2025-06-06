﻿

using UserService;

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
        services.AddAuthorization(configuration);
        services.AddUserServices(configuration);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
      
    }
  
    private static IServiceCollection AddAuthorization(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddAuthorization();
        return service;
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
