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

        await dbContext.SaveChangesAsync();

        using DataProtectionKeysContext dataProtectionKeysContext = services.GetRequiredService<DataProtectionKeysContext>();

        await dataProtectionKeysContext.Database.MigrateAsync();
    }
}
