using Courses.Application.Abstractions.Data;
using Courses.Domain.Shared;
using Courses.Infrastructure.Database.Read;
using Courses.Infrastructure.Database.Write.Configuration;
using Kernel.Messaging.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database.Write;

public sealed class WriteDbContext : AppDbContextBase, IWriteDbContext
{
    private readonly IMediator _mediator;
    private bool _isSaving;

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
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DependencyInjection).Assembly,
            type => type.Namespace is not null &&
                type.Namespace.StartsWith(
                    typeof(WriteDbContext).Namespace!,
                    StringComparison.InvariantCultureIgnoreCase));

        modelBuilder.HasDefaultSchema(SchemaNames.Write);
        modelBuilder.AddTransactionalOutboxEntities();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_isSaving)
        {
            throw new InvalidOperationException(
                "Nested SaveChanges detected!");
        }

        _isSaving = true;

        try
        {
            await DispatchDomainEvents(this, cancellationToken);

            return await base.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _isSaving = false;
        }
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
