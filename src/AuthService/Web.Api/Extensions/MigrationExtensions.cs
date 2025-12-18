using System.Threading;
using Domain.AuthUsers;
using Domain.AuthUsers.Primitives;
using Domain.Permissions;
using Domain.Roles;
using Infrastructure.Database;
using Kernel.Auth.AuthTypes;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        using WriteDbContext dbContext = services.GetRequiredService<WriteDbContext>();
        await dbContext.Database.MigrateAsync();

        if (!await dbContext.Roles.AnyAsync())
        {
            Role userRole = Role.Create("user").Value;
            await dbContext.Roles.AddAsync(userRole);
        }
        AuthUser? user = await dbContext.Users
            .Include(user => user.Roles)
            .FirstOrDefaultAsync(user => user.IdentityId == new IdentityProviderId("asdasdfd"));

        user.AddPermission(new Permission(
            EffectType.Allow,
            ActionType.Create,
            ResourceType.Course,
            ResourceId.Wildcard));

        await dbContext.SaveChangesAsync();
    }
}
