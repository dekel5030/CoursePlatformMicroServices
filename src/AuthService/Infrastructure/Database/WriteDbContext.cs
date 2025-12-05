using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.Roles;
using Infrastructure.DomainEvents;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public class WriteDbContext
    : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>, IWriteDbContext
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public DbSet<AuthUser> DomainUsers { get; set; }
    public DbSet<Role> DomainRoles { get; set; }

    public WriteDbContext(
        DbContextOptions<WriteDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher) : base(options)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
        modelBuilder.AddTransactionalOutboxEntities();
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

        foreach (IHasDomainEvents entity in entities)
        {
            entity.ClearDomainEvents();
        }

        return _domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
