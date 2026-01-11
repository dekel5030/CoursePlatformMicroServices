using System.Reflection;
using Auth.Api.Endpoints;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Auth.Api.Extensions;

internal static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }

    public static RouteHandlerBuilder HasPermission(this RouteHandlerBuilder app, string permission)
    {
        return app.RequireAuthorization(permission);
    }

    public static RouteHandlerBuilder WithMetadata<TResponse>(
        this RouteHandlerBuilder builder,
        string tag,
        string name,
        string summary,
        string description,
        bool requiresAuth = true)
    {
        var result = builder
            .WithTags(tag)
            .WithName(name)
            .WithSummary(summary)
            .WithDescription(description)
            .Produces<TResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        if (requiresAuth)
        {
            result = result
                .RequireAuthorization()
                .ProducesProblem(StatusCodes.Status401Unauthorized);
        }

        return result.ProducesProblem(StatusCodes.Status409Conflict);
    }
}
