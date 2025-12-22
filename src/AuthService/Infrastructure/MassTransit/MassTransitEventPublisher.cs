using Auth.Application.Abstractions.MessageQueue;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.MassTransit;

internal sealed class MassTransitEventPublisher : IEventPublisher
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

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        _logger.LogInformation("Publishing event of type {EventType}: {Event}", @event.GetType(), @event);
        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}
