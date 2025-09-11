using Application.Abstractions.Messaging;
using Domain.Orders.Events;
using Orders.Contracts;

namespace Application.Orders.DomainEvents;

public sealed class OrderSubmittedDomainEventHandler : IDomainEventHandler<OrderSubmittedDomainEvent>
{
    private readonly IEventPublisher _publisher;

    public OrderSubmittedDomainEventHandler(IEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task Handle(OrderSubmittedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        OrderSubmitted contract = new OrderSubmitted(
            domainEvent.Id.Value.ToString(),
            domainEvent.EntityVersion,
            domainEvent.Status.ToString());

        await _publisher.Publish(contract, cancellationToken);
    }
}