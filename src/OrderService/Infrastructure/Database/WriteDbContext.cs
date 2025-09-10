using Application.Abstractions.Data;
using Domain.Orders;
using Domain.Products;
using Domain.Users;
using Infrastructure.DomainEvents;
using Infrastructure.VersionedEntity;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.VersionedEntity;

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
        modelBuilder.AddShadowVersion();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(this, cancellationToken);
        UpdateVersionedEntities();

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
        const string VersionFieldName = "Version";

        foreach (var versionedEntity in ChangeTracker.Entries<IVersionedEntity>())
        {
            var property = versionedEntity.Property(VersionFieldName);

            switch (versionedEntity.State)
            {
                case EntityState.Added:
                    if (property.CurrentValue is null)
                        property.CurrentValue = 1L;
                    break;

                case EntityState.Modified:
                    var original = property.OriginalValue as long? ?? 0L;
                    property.CurrentValue = original + 1L;
                    break;
            }
        }
    }
}