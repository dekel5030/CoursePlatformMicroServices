using Kernel.EventBus;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.MassTransit;

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

    public async Task PublishAsync<TEvent>(TEvent message, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        _logger.LogInformation("Publishing event of type {EventType}: {Event}", message.GetType(), message);
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}
