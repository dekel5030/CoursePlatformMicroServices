using Auth.Application.Abstractions.Data;
using Auth.Infrastructure.DomainEvents;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Database;

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

        var domainEvents = entities
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        foreach (Entity entity in entities)
        {
            entity.ClearDomainEvents();
        }

        return _domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}