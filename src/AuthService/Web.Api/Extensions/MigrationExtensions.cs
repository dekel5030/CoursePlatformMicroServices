using Application.Abstractions.Identity;
using Domain.Roles;
using Infrastructure.Database;
using Infrastructure.Identity.Managers;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        using var readDb = services.GetRequiredService<ReadDbContext>();
        await readDb.Database.MigrateAsync();

        using WriteDbContext dbContext = services.GetRequiredService<WriteDbContext>();
        await dbContext.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<IRoleManager<Role>>();
        if (!roleManager.Roles.Any())
        {
            var roleTypes = Enum.GetValues<RoleType>();
            foreach (var role in roleTypes)
            {
                var domainRole = Role.Create(role.ToString());

                if (!domainRole.IsSuccess)
                {
                    throw new InvalidOperationException($"Failed to create role {role}: {domainRole.Error}");
                }

                await roleManager.AddRoleAsync(domainRole.Value);
            }
        }
        await dbContext.SaveChangesAsync();

        using DataProtectionKeysContext dataProtectionKeysContext = services.GetRequiredService<DataProtectionKeysContext>();

        await dataProtectionKeysContext.Database.MigrateAsync();
    }
}
