using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Orders.Events;

namespace Application.Orders.DomainEvents;

public sealed class OrderSubmittedDomainEventHandler : IDomainEventHandler<OrderSubmitted>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IEventPublisher _publisher;

    public OrderSubmittedDomainEventHandler(IApplicationDbContext dbContext, IEventPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public Task Handle(OrderSubmitted @event, CancellationToken cancellationToken = default)
    {
        // Map from domain event to ingetration event and save to outbox table
        _publisher.Publish(@event, cancellationToken);
        return Task.CompletedTask;
    }
}
