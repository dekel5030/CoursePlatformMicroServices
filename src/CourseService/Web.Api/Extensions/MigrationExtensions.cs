using Courses.Infrastructure.Database.Read;
using Courses.Infrastructure.Database.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Courses.Api.Extensions;

internal static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using WriteDbContext writeDbContext =
            scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        writeDbContext.Database.Migrate();

        using ReadDbContext readDbContext =
            scope.ServiceProvider.GetRequiredService<ReadDbContext>();
        readDbContext.Database.Migrate();
    }
}
