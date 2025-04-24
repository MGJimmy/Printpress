using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Printpress.API.Middlewares;
using Printpress.Application;
using Swashbuckle.AspNetCore.SwaggerGen;
using Printpress.Infrastructure;
using SecurityProvider;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Printpress.Domain;

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
        services.AddAuthentication(configuration);
        services.AddAuthorization(configuration);
        services.AddIdentity(configuration);
        services.AddSecurityProvider();

    }
    private static IServiceCollection AddAuthentication(this IServiceCollection service, IConfiguration configuration)
    {
        var jwtSettingsSection = configuration.GetSection("JwtOption");

        service.Configure<JwtOption>(jwtSettingsSection);

        var jwtSettings = jwtSettingsSection.Get<JwtOption>();

        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        service.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
            };
        });

        return service;
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
    private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
      
        services.AddIdentity<AplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 12;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
       
        return services;
    }

}
