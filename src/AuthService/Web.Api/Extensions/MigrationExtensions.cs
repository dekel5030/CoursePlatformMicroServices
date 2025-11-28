using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using WriteDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        dbContext.Database.Migrate();
    }
}
