using Application.Abstractions.Data;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    private static readonly string _readDatabaseConnectionStringName = "ReadDatabase";
    private static readonly string _writeDatabaseConnectionStringName = "WriteDatabase";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReadDatabases(configuration);
        services.AddWriteDatabase(configuration);

        return services;
    }

    private static IServiceCollection AddReadDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReadDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(_readDatabaseConnectionStringName)!;

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IReadDbContext>(serviceProvider => serviceProvider.GetRequiredService<ReadDbContext>());

        return services;
    }

    private static IServiceCollection AddWriteDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(_writeDatabaseConnectionStringName)!;

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IWriteDbContext>(serviceProvider => serviceProvider.GetRequiredService<WriteDbContext>());

        return services;
    }
}
