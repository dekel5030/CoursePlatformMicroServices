using Application.Abstractions.Data;
using Domain.Courses;
using Domain.Lessons;
using Infrastructure.DomainEvents;
using Kernel;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class WriteDbContext(
    DbContextOptions<WriteDbContext> options,
    IDomainEventsDispatcher dispatcher)
    : DbContext(options), IWriteDbContext
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly);

        modelBuilder.AddTransactionalOutboxEntities();
        AssignConcurrencyToken(modelBuilder);
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

        return dispatcher.DispatchAsync(domainEvents, cancellationToken);
    }

    private void AssignConcurrencyToken(ModelBuilder modelBuilder)
    {
        foreach (var et in modelBuilder.Model.GetEntityTypes()
             .Where(t => typeof(IVersionedEntity).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(et.ClrType)
                .Property<long>(nameof(IVersionedEntity.EntityVersion))
                .IsConcurrencyToken()
                .HasColumnName("entity_version");
        }
    }
}