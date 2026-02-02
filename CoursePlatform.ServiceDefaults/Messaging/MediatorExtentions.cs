using System.Reflection;
using System.Text;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CoursePlatform.ServiceDefaults.Messaging;

public static class MediatorExtentions
{
    public static IServiceCollection AddMediator<TMarker>(
        this IServiceCollection services)
    {
        Assembly assembly = typeof(TMarker).Assembly;
        string serviceName = assembly.GetName().Name!;

        services.AddScoped<IMediator, Mediator>();

        services.AddRequestHandlers(assembly, serviceName);
        services.AddEventHandlers(assembly, serviceName);

        return services;
    }

    public static IServiceCollection AddOpenBehavior(
        this IServiceCollection services,
        Type behaviorType)
    {
        if (!behaviorType.IsGenericTypeDefinition)
        {
            throw new ArgumentException($"{behaviorType.Name} must be a generic type definition (e.g. typeof(LoggingBehavior<,>))");
        }

        services.AddScoped(typeof(IPipelineBehavior<,>), behaviorType);

        return services;
    }

    private static IServiceCollection AddRequestHandlers(
        this IServiceCollection services,
        Assembly assembly,
        string serviceName)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        PrintLog("Request Handlers", serviceName, handlerTypes);

        services.Scan(selector =>
            selector
                .FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .Where(t => t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))),
                    publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;
    }

    private static IServiceCollection AddEventHandlers(
        this IServiceCollection services,
        Assembly assembly,
        string serviceName)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
            .ToList();

        PrintLog("Event Handlers", serviceName, handlerTypes);

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

    private static void PrintLog(string handlerType, string? serviceName, List<Type> types)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"--- [Mediator Registration] ---");
        sb.AppendLine($"Service: {serviceName}");
        sb.AppendLine($"Category: {handlerType}");
        sb.AppendLine($"Count: {types.Count}");

        if (types.Count > 0)
        {
            sb.AppendLine("Classes Registered:");
            foreach (Type type in types)
            {
                sb.AppendLine($" > {type.Name}");
            }
        }
        else
        {
            sb.AppendLine("(!) No handlers found in this assembly.");
        }
        sb.AppendLine("-------------------------------");

        Console.WriteLine(sb.ToString());
    }
}
