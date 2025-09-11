using Application.Abstractions.Data;
using Domain.Orders;
using Domain.Products;
using Domain.Users;
using Infrastructure.DomainEvents;
using Kernel;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class WriteDbContext(
    DbContextOptions<WriteDbContext> options,
    IDomainEventsDispatcher dispatcher)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<LineItem> LineItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly);
        modelBuilder.HasDefaultSchema(Schemas.Default);

        modelBuilder.AddTransactionalOutboxEntities();
        AssignConcurrencyToken(modelBuilder);
    }

    /// <summary>
    /// First , it updates the version of all versioned entities.
    /// then it dispatches all domain events.
    /// and finally, it saves changes to the database.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateVersionedEntities();

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

    private void UpdateVersionedEntities()
    {
        foreach (var versionedEntity in ChangeTracker.Entries<IVersionedEntity>())
        {
            var property = versionedEntity.Property(entity => entity.EntityVersion);

            switch (versionedEntity.State)
            {
                case EntityState.Added:
                    property.CurrentValue = 1L;
                    break;

                case EntityState.Modified:
                    var original = property.OriginalValue;
                    property.CurrentValue = original + 1L;
                    break;
            }
        }
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