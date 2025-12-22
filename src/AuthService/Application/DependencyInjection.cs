using CoursePlatform.ServiceDefaults.Messaging;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.Scan(selector => selector
        //        .FromAssemblies(typeof(DependencyInjection).Assembly)
        //        .AddClasses(classes => classes
        //        .AssignableTo(typeof(IRequestHandler<,>)))
        //        .AsImplementedInterfaces()
        //        .WithScopedLifetime());

        services.Scan(selector => selector
                .FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes
                    .Where(t => t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))),
                    publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddIntegrationEventHandlers();
        //services.AddValidators();

        services.AddScoped<IMediator, Mediator>();

        return services;
    }

    private static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services)
    {
        // Integration event handlers will be added here
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // Validators can be added here
        return services;
    }
}
