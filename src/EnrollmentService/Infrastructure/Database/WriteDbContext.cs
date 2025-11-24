using Application.Abstractions.Data;
using Domain.Enrollments;
using Domain.Users;
using Domain.Courses;
using Infrastructure.DomainEvents;
using Kernel;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public class WriteDbContext : DbContext, IWriteDbContext
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public WriteDbContext(
        DbContextOptions<WriteDbContext> options,
        IDomainEventsDispatcher domainEventsDispatcher) : base(options)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<KnownUser> KnownUsers => Set<KnownUser>();
    public DbSet<KnownCourse> KnownCourses => Set<KnownCourse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Default);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateEntityVersions();

        var entities = ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        await _domainEventsDispatcher.DispatchAsync(entities, cancellationToken);

        return result;
    }

    private void UpdateEntityVersions()
    {
        foreach (var entry in ChangeTracker.Entries<IVersionedEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IVersionedEntity.EntityVersion)).CurrentValue = 1L;
            }
            else if (entry.State == EntityState.Modified)
            {
                var currentVersion = (long)entry.Property(nameof(IVersionedEntity.EntityVersion)).CurrentValue!;
                entry.Property(nameof(IVersionedEntity.EntityVersion)).CurrentValue = currentVersion + 1;
            }
        }
    }
}
