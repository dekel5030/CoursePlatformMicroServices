using Application.Abstractions.Data;
using Domain.Orders;
using Infrastructure.DomainEvents;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel;
using System.Text.Json;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Order> Orders { get; set; }

    public DbSet<LineItem> LineItems { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.HasDefaultSchema(Schemas.Default);
    }
}

internal sealed class DomainEventDispatcherInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventsDispatcher _dispatcher;

    public DomainEventDispatcherInterceptor(IDomainEventsDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        DbContext dbContext = eventData.Context
            ?? throw new ArgumentNullException(nameof(dbContext));

        await DispatchDomainEvents(dbContext, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(
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

        await _dispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}

internal sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext dbContext = eventData.Context
            ?? throw new ArgumentNullException(nameof(dbContext));

        await InsertOutboxMessages(dbContext, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task InsertOutboxMessages(
        DbContext dbContext, 
        CancellationToken cancellationToken = default)
    {
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;

        IEnumerable<IDomainEvent> domainEvents = dbContext.ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity => entity.DomainEvents);
        var first = domainEvents.First();
        var json = JsonSerializer.Serialize(first);
        List<OutboxMessage> outboxMessages = domainEvents
            .Select(domainEvent => new OutboxMessage { 
                Id = Guid.NewGuid(), 
                OccurredAt = utcNow,
                Type = domainEvent.GetType().AssemblyQualifiedName!, 
                Content = JsonSerializer.Serialize(domainEvent) })
            .ToList();

        await dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);
    }
}