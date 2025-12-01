using Application.Abstractions.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MassTransit;

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

    public async Task Publish<T>(T message, CancellationToken cancellationToken = default)
        where T : notnull
    {
        _logger.LogInformation("Publishing message of type {MessageType}: {Message}", message.GetType(), message);
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}