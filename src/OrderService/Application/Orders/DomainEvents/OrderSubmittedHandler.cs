using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Orders.Events;

namespace Application.Orders.DomainEvents;

public sealed class OrderSubmittedHandler : IDomainEventHandler<OrderSubmitted>
{
    private readonly IApplicationDbContext _dbContext;

    public OrderSubmittedHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(OrderSubmitted e, CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
