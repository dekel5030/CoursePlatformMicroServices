using System.Reflection;
using Kernel.EventBus;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Users.Infrastructure.Database;

namespace Users.Infrastructure.MassTransit;

internal static class MassTransitExtensions
{
    internal static IServiceCollection AddMassTransitInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumers(typeof(DependencyInjection).Assembly);

            config.RegisterApplicationConsumers();

            config.AddEntityFrameworkOutbox<WriteDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
                o.QueryDelay = TimeSpan.FromSeconds(30);
            });


            config.AddConfigureEndpointsCallback((ctx, endpointName, endpointCfg) =>
            {
                endpointCfg.UseEntityFrameworkOutbox<WriteDbContext>(ctx);
                endpointCfg.UseMessageRetry(r =>
                {
                    r.Handle<InvalidOperationException>();
                    r.Intervals(
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(40));
                });
            });

            config.UsingRabbitMq((context, busConfig) =>
            {
                string connectionString = configuration
                    .GetConnectionString(DependencyInjection.RabbitMqConnectionStringName)!;

                busConfig.Host(new Uri(connectionString!), h => { });
                busConfig.ConfigureEndpoints(context);
            });

            config.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "masstransit";
                options.MinimalFailureStatus = HealthStatus.Unhealthy;
                options.Tags.Add("ready");
            });
        });

        services.AddScoped<IEventBus, MassTransitEventPublisher>();

        return services;
    }

    private static void RegisterApplicationConsumers(
        this IBusRegistrationConfigurator registrationConfigurator)
    {
        Assembly applicationAssembly = typeof(Application.DependencyInjection).Assembly;

        var eventTypes = applicationAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces())
            .Where(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IEventConsumer<>))
            .Select(i => i.GetGenericArguments()[0])
            .Distinct()
            .ToList();

        LogBridgedConsumers(applicationAssembly.GetName().Name, eventTypes);

        foreach (Type? eventType in eventTypes)
        {
            Type bridgeType = typeof(GenericConsumerBridge<>).MakeGenericType(eventType);
            registrationConfigurator.AddConsumer(bridgeType);
        }
    }

    private static void LogBridgedConsumers(string? assemblyName, List<Type> eventTypes)
    {

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("--- [MassTransit Bridge Registration] ---");
        sb.AppendLine($"Source: {assemblyName ?? "Unknown"}");
        sb.AppendLine($"Total Bridged Events: {eventTypes.Count}");

        if (eventTypes.Count > 0)
        {
            sb.AppendLine("Events Subscribed (via GenericBridge):");
            foreach (Type type in eventTypes)
            {
                sb.AppendLine($" > {type.Name}");
            }
        }
        else
        {
            sb.AppendLine("(!) No event consumers found to bridge.");
        }
        sb.AppendLine("-----------------------------------------");

        Console.WriteLine(sb.ToString());
    }
}
