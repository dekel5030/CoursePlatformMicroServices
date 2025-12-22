using Microsoft.EntityFrameworkCore;
using Users.Infrastructure.Database;

namespace Users.Api.Extensions;

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