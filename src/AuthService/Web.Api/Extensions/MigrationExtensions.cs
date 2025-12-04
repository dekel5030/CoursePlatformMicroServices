using Domain.Roles;
using Infrastructure.Database;
using Infrastructure.Identity;
using Kernel.Auth.AuthTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        using WriteDbContext dbContext = services.GetRequiredService<WriteDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<ApplicationIdentityRole>>();
        if (!roleManager.Roles.Any())
        {
            var roleTypes = Enum.GetValues<RoleType>();
            foreach (var role in roleTypes)
            {
                var domainRole = Role.Create(role.ToString());
                var applicationRole = new ApplicationIdentityRole(domainRole);
                await roleManager.CreateAsync(applicationRole);
            }
        }

        using DataProtectionKeysContext dataProtectionKeysContext = services.GetRequiredService<DataProtectionKeysContext>();

        await dataProtectionKeysContext.Database.MigrateAsync();
    }
}
