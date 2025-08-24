using Application.Abstractions.Messaging;
using Domain.Orders.Events;

namespace Application.Orders.DomainEvents;

public sealed class OrderCreatedHandler : IDomainEventHandler<OrderDraftOpened>
{
    public Task Handle(OrderDraftOpened domainEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
