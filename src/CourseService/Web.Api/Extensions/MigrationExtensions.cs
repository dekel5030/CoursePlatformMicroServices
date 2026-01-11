using Courses.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Courses.Api.Extensions;

internal static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using WriteDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        dbContext.Database.Migrate();
    }
}
