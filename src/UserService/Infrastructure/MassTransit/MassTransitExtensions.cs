using Kernel.EventBus;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            config.RegisterApplicationConsumers(services);

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
                var connectionString = configuration
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
            this IBusRegistrationConfigurator registrationConfigurator,
            IServiceCollection services)
    {
        var applicationAssembly = typeof(Application.DependencyInjection).Assembly;

        var consumerDefinitions = applicationAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (concreteType, interfaceType) => new { concreteType, interfaceType })
            .Where(x => x.interfaceType.IsGenericType &&
                        x.interfaceType.GetGenericTypeDefinition() == typeof(IEventConsumer<>))
            .ToList();

        foreach (var entry in consumerDefinitions)
        {
            var eventType = entry.interfaceType.GetGenericArguments()[0];

            var closedBridgeType = typeof(GenericConsumerBridge<>).MakeGenericType(eventType);

            registrationConfigurator.AddConsumer(closedBridgeType);

            services.TryAddScoped(entry.interfaceType, entry.concreteType);
        }
    }
}
