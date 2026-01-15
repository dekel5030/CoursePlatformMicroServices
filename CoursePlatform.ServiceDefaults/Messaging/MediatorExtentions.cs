using System.Reflection;
using CoursePlatform.ServiceDefaults.Messaging.Behaviors;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CoursePlatform.ServiceDefaults.Messaging;

public static class MediatorExtentions
{
    public static IServiceCollection AddMediator<TMarker>(
        this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        services.Scan(selector => 
            selector
                .FromAssemblies(typeof(TMarker).Assembly)
                .AddClasses(classes => classes
                    .Where(t => t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))),
                    publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddEventHandlers(typeof(TMarker).Assembly);

        return services;
    }

    private static IServiceCollection AddEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.Scan(selector => selector
                .FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .Where(t => t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IEventHandler<>))),
                    publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;

    }
}
