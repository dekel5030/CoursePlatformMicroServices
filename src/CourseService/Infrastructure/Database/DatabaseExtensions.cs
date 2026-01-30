using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Infrastructure.Database.Read;
using Courses.Infrastructure.Database.Repositories;
using Courses.Infrastructure.Database.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Infrastructure.Database;

internal static class DatabaseExtensions
{
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
                        HistoryRepository.DefaultTableName, SchemaNames.Write);
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IWriteDbContext>(sp => sp.GetRequiredService<WriteDbContext>());
        services.AddScoped<IReadDbContext, ReadDbContext>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WriteDbContext>());
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IFeaturedCoursesRepository, FeaturedCoursesRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IModuleRepository, ModulesRepository>();
        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

        return services;
    }
}
