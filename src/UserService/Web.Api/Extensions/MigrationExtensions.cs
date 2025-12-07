using Infrastructure.Database;
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

        using var writeDb = services.GetRequiredService<WriteDbContext>();
        await writeDb.Database.MigrateAsync();
    }
}