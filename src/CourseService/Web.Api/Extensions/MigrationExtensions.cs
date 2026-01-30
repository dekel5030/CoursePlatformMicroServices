using Courses.Infrastructure.Database.Write;
using Microsoft.EntityFrameworkCore;

namespace Courses.Api.Extensions;

internal static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using WriteDbContext writeDbContext =
            scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        writeDbContext.Database.Migrate();
    }
}
