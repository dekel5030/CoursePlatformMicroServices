using Application.Abstractions.Data;
using Infrastructure.DomainEvents;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class WriteDbContext : AppDbContextBase, IWriteDbContext, IUnitOfWork
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public WriteDbContext(
        DbContextOptions<WriteDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher,
        IServiceProvider serviceProvider)
        : base(options)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(this, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    private Task DispatchDomainEvents(
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

        return _domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}