using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.Permissions;
using Domain.Roles;
using Infrastructure.DomainEvents;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public class AuthDbContext : IdentityDbContext<AuthUser, Role, Guid>, IWriteDbContext, IReadDbContext
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public AuthDbContext(
        DbContextOptions<AuthDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher) : base(options)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);

        modelBuilder.AddTransactionalOutboxEntities();

        base.OnModelCreating(modelBuilder);
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
        IEnumerable<IHasDomainEvents> entities = dbContext.ChangeTracker.Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity);

        List<IDomainEvent> domainEvents = entities
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        foreach (Entity entity in entities)
        {
            entity.ClearDomainEvents();
        }

        return _domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
