using Application.Abstractions.Data;
using Domain.Enrollments;
using Domain.Users;
using Domain.Courses;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ReadDbContext : DbContext, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<KnownUser> KnownUsers { get; set; }
    public DbSet<KnownCourse> KnownCourses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Default);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
