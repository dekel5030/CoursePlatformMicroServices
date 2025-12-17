using Infrastructure.Database;
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

        await dbContext.SaveChangesAsync();
    }
}
