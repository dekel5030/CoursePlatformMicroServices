using CoursePlatform.ServiceDefaults.Auth;
using CoursePlatform.ServiceDefaults.Messaging;
using Courses.Application.Lessons.Commands.ReorderLessons;
using Courses.Application.Modules.Commands.ReorderModules;
using Courses.Infrastructure.Ai;
using Courses.Infrastructure.Behaviors;
using Courses.Infrastructure.Cache;
using Courses.Infrastructure.Database;
using Courses.Infrastructure.MassTransit;
using Courses.Infrastructure.Storage;
using Kernel;
using Kernel.Messaging.Abstractions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
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
            .AddHealthChecksInternal()
            .AddAuthenticationInternal()
            .AddInternalAuth(authUrl)
            .AddAiProvider()
            .AddCacheService();
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddStorage();
        services.AddScoped<IPipelineBehavior<ReorderLessonsCommand, Result>, ReorderLessonsLockBehavior<ReorderLessonsCommand, Result>>();
        services.AddScoped<IPipelineBehavior<ReorderModulesCommand, Result>, ReorderModulesLockBehavior<ReorderModulesCommand, Result>>();

        return services;
    }

    private static IServiceCollection AddHealthChecksInternal(this IServiceCollection services)
    {
        //services
        //    .AddHealthChecks()
        //    .AddNpgSql(configuration.GetConnectionString("Database")!);

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services)
    {
        return services;
    }
}
