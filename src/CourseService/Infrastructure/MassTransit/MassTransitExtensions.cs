using System.Reflection;
using Courses.Infrastructure.Database;
using Kernel.EventBus;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Courses.Infrastructure.MassTransit;

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
                    .GetConnectionString(DependencyInjection.RabbitMqConnectionSection)!;

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
        Assembly applicationAssembly = typeof(Application.AssemblyMarker).Assembly;

        var consumerDefinitions = applicationAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (concreteType, interfaceType) => new { concreteType, interfaceType })
            .Where(x => x.interfaceType.IsGenericType &&
                        x.interfaceType.GetGenericTypeDefinition() == typeof(IEventConsumer<>))
            .ToList();

        foreach (var entry in consumerDefinitions)
        {
            Type eventType = entry.interfaceType.GetGenericArguments()[0];

            Type closedBridgeType = typeof(GenericConsumerBridge<>).MakeGenericType(eventType);

            registrationConfigurator.AddConsumer(closedBridgeType);
        }
    }
}
