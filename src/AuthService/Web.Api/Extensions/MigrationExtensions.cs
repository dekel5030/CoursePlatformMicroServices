using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using AuthDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        dbContext.Database.Migrate();
    }
}
