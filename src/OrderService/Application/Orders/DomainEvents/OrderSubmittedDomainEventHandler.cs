using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Orders.Events;

namespace Application.Orders.DomainEvents;

public sealed class OrderSubmittedDomainEventHandler : IDomainEventHandler<OrderSubmitted>
{
    private readonly IEventPublisher _publisher;
    private readonly IApplicationDbContext _dbContext;

    public OrderSubmittedDomainEventHandler(IEventPublisher publisher, IApplicationDbContext dbContext)
    {
        _publisher = publisher;
        _dbContext = dbContext;
    }

    public async Task Handle(OrderSubmitted @event, CancellationToken cancellationToken = default)
    {
        // Map from domain event to ingetration event and save to outbox table
        await _publisher.Publish(@event, cancellationToken);
        await _dbContext.SaveChangesAsync();
    }
}
 