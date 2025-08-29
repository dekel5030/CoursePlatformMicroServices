using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel;

namespace Infrastructure.DomainEvents;

internal sealed class DomainEventDispatcherInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventsDispatcher _dispatcher;

    public DomainEventDispatcherInterceptor(IDomainEventsDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        DbContext dbContext = eventData.Context
            ?? throw new ArgumentNullException(nameof(dbContext));

        await DispatchDomainEvents(dbContext, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(
        DbContext dbContext, 
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Entity> entities = dbContext.ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity);

        List<IDomainEvent> domainEvents = entities
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        foreach (Entity entity in entities)
        {
            entity.ClearDomainEvents();
        }

        await _dispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
