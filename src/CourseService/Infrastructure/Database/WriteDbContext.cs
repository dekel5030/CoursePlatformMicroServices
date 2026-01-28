using Courses.Application.Abstractions.Data;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Courses.Domain.Shared;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(SchemaNames.Write);
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
        var entities = dbContext.ChangeTracker.Entries<IEntity>()
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(entity => entity.DomainEvents)
            .Distinct()
            .ToList();

        foreach (IEntity entity in entities)
        {
            entity.ClearDomainEvents();
        }

        IEnumerable<Task> tasks = domainEvents
            .Select(domainEvent => _mediator.Publish(domainEvent, cancellationToken));

        return Task.WhenAll(tasks);
    }
}
