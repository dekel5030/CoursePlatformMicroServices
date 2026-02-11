using Microsoft.EntityFrameworkCore;
using Users.Infrastructure.Database;

namespace Users.Api.Extensions;

internal static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        using ReadDbContext readDb = services.GetRequiredService<ReadDbContext>();
        await readDb.Database.MigrateAsync();

        using WriteDbContext writeDb = services.GetRequiredService<WriteDbContext>();
        await writeDb.Database.MigrateAsync("20260119070353_AvatarUrl_Optional");
    }
}
