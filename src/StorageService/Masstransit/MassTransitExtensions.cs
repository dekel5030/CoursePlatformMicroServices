using Kernel.EventBus;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace StorageService.Masstransit;

internal static class MassTransitExtensions
{
    private const string SectionName = "RabbitMQ";

    internal static IServiceCollection AddMassTransitInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.UsingRabbitMq((context, busConfig) =>
            {
                string connectionString = configuration.GetConnectionString(SectionName)
                    ?? throw new InvalidOperationException("RabbitMQ connection string not found");

                busConfig.Host(new Uri(connectionString), h =>
                {
                    h.PublisherConfirmation = true;
                });

                busConfig.ConfigureEndpoints(context);
            });

            config.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "rabbitmq";
                options.MinimalFailureStatus = HealthStatus.Unhealthy;
            });
        });

        services.AddScoped<IEventBus, MassTransitEventPublisher>();

        return services;
    }
}