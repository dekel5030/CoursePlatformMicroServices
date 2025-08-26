using Application.Abstractions.Data;
using Domain.Orders;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Order> Orders { get; set; }

    public DbSet<LineItem> LineItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(cancellationToken);

        return result;
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = ChangeTracker.Entries()
            .Select(e => e.Entity)
            .OfType<Entity>()
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        var events = domainEntities
            .SelectMany(e => e.DomainEvents.ToList())
            .ToList();

        domainEntities.ForEach(e => e.ClearDomainEvents());

        await domainEventsDispatcher.DispatchAsync(events, cancellationToken);
    }
}
