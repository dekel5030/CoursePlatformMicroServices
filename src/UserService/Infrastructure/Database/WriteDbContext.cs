using Kernel;
using Kernel.Messaging.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Data;
using Users.Domain.Users;

namespace Users.Infrastructure.Database;

public sealed class WriteDbContext(
    DbContextOptions<WriteDbContext> options,
    IMediator mediator)
        : DbContext(options), IWriteDbContext
{
    public DbSet<User> Users { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(this, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
        modelBuilder.AddTransactionalOutboxEntities();
    }

    private Task DispatchDomainEvents(
        DbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var entities = dbContext.ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        foreach (Entity entity in entities)
        {
            entity.ClearDomainEvents();
        }

        return mediator.Publish(domainEvents, cancellationToken);
    }
}
