using System.Reflection;
using EnrollmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data;

public class EnrollmentDbContext : DbContext
{
    public EnrollmentDbContext(DbContextOptions<EnrollmentDbContext> options)
        : base(options) { }

    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<KnownUser> KnownUsers { get; set; }
    public DbSet<KnownCourse> KnownCourses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<KnownCourse>().HasKey(x => x.CourseId);
        modelBuilder.Entity<KnownUser>().HasKey(x => x.UserId);

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.AggregateVersion)
            .IsConcurrencyToken();

        base.OnModelCreating(modelBuilder);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var e in ChangeTracker.Entries<Enrollment>())
        {
            if (e.State == EntityState.Added)
                e.Entity.AggregateVersion = 0;
            else if (e.State == EntityState.Modified)
                e.Entity.AggregateVersion++;
        }
        
        return base.SaveChangesAsync(ct);
    }
}