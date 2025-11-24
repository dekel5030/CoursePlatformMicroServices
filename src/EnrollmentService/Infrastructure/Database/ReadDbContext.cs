using Application.Abstractions.Data;
using Domain.Enrollments;
using Domain.Users;
using Domain.Courses;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ReadDbContext : DbContext, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<KnownUser> KnownUsers => Set<KnownUser>();
    public DbSet<KnownCourse> KnownCourses => Set<KnownCourse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Default);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
