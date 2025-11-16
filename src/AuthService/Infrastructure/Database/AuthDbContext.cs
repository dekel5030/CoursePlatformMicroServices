using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.AuthUsers.Primitives;
using Domain.Permissions.Primitives;
using Domain.Roles;
using Domain.Roles.Primitives;
using Infrastructure.DomainEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public class AuthDbContext : DbContext, IWriteDbContext, IReadDbContext
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public AuthDbContext(
        DbContextOptions<AuthDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher) : base(options)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    public DbSet<AuthUser> AuthUsers { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);

        modelBuilder.AddTransactionalOutboxEntities();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeEmails();
        UpdateTimestamps();

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

        return _domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }

    private void NormalizeEmails()
    {
        foreach (var entry in ChangeTracker.Entries<AuthUser>())
        {
            if ((entry.State == EntityState.Added || entry.State == EntityState.Modified) &&
                !string.IsNullOrWhiteSpace(entry.Entity.Email))
            {
                entry.Property(nameof(AuthUser.Email)).CurrentValue = 
                    entry.Entity.Email.ToLowerInvariant();
            }
        }
    }

    private void UpdateTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<AuthUser>())
        {
            if (entry.State == EntityState.Modified && entry.Properties.Any(p => p.IsModified))
            {
                entry.Property(nameof(AuthUser.UpdatedAt)).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
