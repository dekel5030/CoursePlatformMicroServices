using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Infrastructure.Database;

internal static class DatabaseExtensions
{
    private const string ReadDatabaseConnectionSection = "ReadDatabase";
    private const string WriteDatabaseConnectionSection = "WriteDatabase";

    internal static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(WriteDatabaseConnectionSection)
                ?? throw new InvalidOperationException("Database connection string not found");

            options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName);
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddDbContext<ReadDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(ReadDatabaseConnectionSection)
                ?? throw new InvalidOperationException("Database connection string not found");

            options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName);
                })
                .UseSnakeCaseNamingConvention()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IWriteDbContext>(sp => sp.GetRequiredService<WriteDbContext>());
        services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<ReadDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WriteDbContext>());
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IFeaturedCoursesRepository, FeaturedCoursesRepository>();

        return services;
    }
}
