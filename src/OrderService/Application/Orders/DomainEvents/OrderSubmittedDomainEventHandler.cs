using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Orders.Events;

namespace Application.Orders.DomainEvents;

public sealed class OrderSubmittedDomainEventHandler : IDomainEventHandler<OrderSubmitted>
{
    private readonly IApplicationDbContext _dbContext;

    public OrderSubmittedDomainEventHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(OrderSubmitted @event, CancellationToken cancellationToken = default)
    {
        // Map from domain event to ingetration event and save to outbox table
        return Task.CompletedTask;
    }
}
