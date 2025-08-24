using Application.Abstractions.Messaging;
using Domain.Orders.Events;

namespace Application.Orders.DomainEvents;

public sealed class OrderCreatedHandler : IDomainEventHandler<OrderCreated>
{
    public Task Handle(OrderCreated domainEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
