using Application.Orders.DomainEvents;
using MassTransit;

namespace Infrastructure.MassTransit;

internal sealed class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task Publish<T>(T message, CancellationToken cancellationToken = default) 
        where T : notnull
    {
        return _publishEndpoint.Publish(message, cancellationToken);
    }
}