using System.Security.Claims;
using CoursePlatform.ServiceDefaults.Auth;
using Courses.Domain.Enrollments;
using Courses.Infrastructure.Database;
using Courses.Infrastructure.MassTransit;
using Courses.Infrastructure.Storage;
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
            .AddInternalAuth(authUrl);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.Use(async (context, next) =>
        {
            ClaimsPrincipal user = context.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine($"User Authenticated: {user.Identity.Name}");
            }
            else
            {
                Console.WriteLine("User NOT Authenticated");

                string? authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                Console.WriteLine($"Auth Header: {authHeader}");
            }

            await next();
        });

        return app;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddStorage();
        services.AddScoped<EnrollmentManager>();
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
