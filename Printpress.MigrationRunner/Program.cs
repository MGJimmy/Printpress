using Identity.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Printpress.Infrastructure;
using Printpress.MigrationRunner;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")))
                .AddDbContext<IdentityDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("UserConnectionString")))
                .AddIdentity<User, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 7;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders()
                .Services
                .AddScoped<SeedingDbContext>()
                .BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var seedingDbContext = scope.ServiceProvider.GetRequiredService<SeedingDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            dbContext.Database.Migrate();
            identityDbContext.Database.Migrate();

            seedingDbContext.SeedingData();

            dbContext.CurrentUserId = "Seeding";
            dbContext.SaveChanges();

            await IdentitySeeder.SeedAsync(userManager, roleManager);

            Console.WriteLine("Database migrations and default data applied successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while applying migrations: " + ex.Message);
            Console.WriteLine(ex);
            Environment.ExitCode = 1;
        }
    }
}
