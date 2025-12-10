using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.Roles;
using Infrastructure.DomainEvents;
using Infrastructure.Identity;
using Infrastructure.Redis.EventCollector;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public class WriteDbContext
    : IdentityDbContext<ApplicationIdentityUser, ApplicationIdentityRole, Guid>, IWriteDbContext, IUnitOfWork
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;
    private readonly IRoleEventsCollector _roleEventsCollector;

    public DbSet<AuthUser> DomainUsers { get; set; }
    public DbSet<Role> DomainRoles { get; set; }

    DbSet<Role> IWriteDbContext.Roles => DomainRoles;

    public DbSet<AuthUser> AuthUsers => DomainUsers;

    public WriteDbContext(
        DbContextOptions<WriteDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher,
        IRoleEventsCollector roleEventsCollector) : base(options)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
        _roleEventsCollector = roleEventsCollector;
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

        await _roleEventsCollector.FlushAsync(cancellationToken);

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
