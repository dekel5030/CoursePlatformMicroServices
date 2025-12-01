using Domain.Roles;
using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAndSeedAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        using WriteDbContext dbContext = services.GetRequiredService<WriteDbContext>();

        await dbContext.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        string[] roles = { "User", "Admin", "Instructor" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = Role.Create(roleName);
                await roleManager.CreateAsync(role);
            }
        }
    }
}
