using Identity.Service;
using Microsoft.AspNetCore.Identity;

namespace Printpress.MigrationRunner;

internal static class IdentitySeeder
{
    private const string AdminUserName = "admin";
    private const string AdminPassword = "1q2w3E*";
    private const string AdminEmail = "admin@printpress.local";

    public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        await EnsureRoleAsync(roleManager, RoleName.Admin);
        await EnsureRoleAsync(roleManager, RoleName.User);

        var admin = await userManager.FindByNameAsync(AdminUserName);
        if (admin is null)
        {
            admin = new User
            {
                UserName = AdminUserName,
                Email = AdminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User"
            };

            var createResult = await userManager.CreateAsync(admin, AdminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create default admin user: {errors}");
            }

            Console.WriteLine("Default admin user created (username: admin).");
        }
        else
        {
            Console.WriteLine("Default admin user already exists.");
        }

        if (!await userManager.IsInRoleAsync(admin, RoleName.Admin))
        {
            var roleResult = await userManager.AddToRoleAsync(admin, RoleName.Admin);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign Admin role: {errors}");
            }

            Console.WriteLine("Admin role assigned to default admin user.");
        }
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var result = await roleManager.CreateAsync(new IdentityRole
        {
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant()
        });

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create role '{roleName}': {errors}");
        }
    }
}
