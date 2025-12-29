using Kernel.EventBus;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Users.Infrastructure.MassTransit;

internal sealed class MassTransitEventPublisher : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitEventPublisher> _logger;

    public MassTransitEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        _logger.LogInformation("Publishing event of type {EventType}", typeof(TEvent).FullName);
        return _publishEndpoint.Publish(@event, cancellationToken);
    }
}