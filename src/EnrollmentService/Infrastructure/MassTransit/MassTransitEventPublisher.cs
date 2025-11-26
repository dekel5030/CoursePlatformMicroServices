using Application.Abstractions.Messaging;
using MassTransit;

namespace Infrastructure.MassTransit;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : notnull
    {
        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}