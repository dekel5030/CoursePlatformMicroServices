using Courses.Application.Abstractions.Data;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database;

public sealed class WriteDbContext : AppDbContextBase, IWriteDbContext
{
    private readonly IMediator _mediator;

    public WriteDbContext(
        DbContextOptions<WriteDbContext> options,
        IMediator mediator) 
        : base(options)
    {
        _mediator = mediator;
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

        IEnumerable<Task> tasks = domainEvents
            .Select(domainEvent => _mediator.Publish(domainEvent, cancellationToken));
    
        return Task.WhenAll(tasks);
    }
}