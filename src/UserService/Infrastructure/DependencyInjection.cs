using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Abstractions.Context;
using Users.Application.Abstractions.Data;
using Users.Infrastructure.Auth;
using Users.Infrastructure.Database;
using Users.Infrastructure.MassTransit;

namespace Users.Infrastructure;

public static class DependencyInjection
{
    internal const string ReadDatabaseConnectionStringName = "ReadDatabase";
    internal const string WriteDatabaseConnectionStringName = "WriteDatabase";
    internal const string RabbitMqConnectionStringName = "RabbitMq";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddMassTransitInternal(configuration);
        //services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

        //services.ConfigureJwtAuthentication(configuration);
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        //app.UseAuthentication();
        //app.UseAuthorization();

        return app;
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
            string connectionString = configuration.GetConnectionString(ReadDatabaseConnectionStringName)!;

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IReadDbContext>(serviceProvider => serviceProvider.GetRequiredService<ReadDbContext>());

        return services;
    }

    private static IServiceCollection AddWriteDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(WriteDatabaseConnectionStringName)!;

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IWriteDbContext>(serviceProvider => serviceProvider.GetRequiredService<WriteDbContext>());

        return services;
    }
}