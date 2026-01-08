using CoursePlatform.ServiceDefaults.Auth;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.Repositories;
using Courses.Infrastructure.Database;
using Courses.Infrastructure.MassTransit;
using Courses.Infrastructure.Repositories;
using Courses.Infrastructure.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Infrastructure;

public static class DependencyInjection
{
    internal const string ReadDatabaseConnectionSection = "ReadDatabase";
    internal const string WriteDatabaseConnectionSection = "WriteDatabase";
    internal const string RabbitMqConnectionSection = "RabbitMq";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        string authUrl = configuration["services:authservice:https:0"]
              ?? configuration["services:authservice:http:0"] ?? string.Empty;

        return services
            .AddServices()
            .AddDatabases(configuration)
            .AddMassTransitInternal(configuration)
            .AddHealthChecksInternal(configuration)
            .AddAuthenticationInternal(configuration)
            .AddInternalAuth(authUrl);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFeaturedCoursesRepository, FeaturedCoursesRepo>();
        services.AddStorage();
        return services;
    }

    private static IServiceCollection AddHealthChecksInternal(this IServiceCollection services, IConfiguration configuration)
    {
        //services
        //    .AddHealthChecks()
        //    .AddNpgSql(configuration.GetConnectionString("Database")!);

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}
