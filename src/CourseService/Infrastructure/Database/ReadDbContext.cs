using Courses.Application.Abstractions.Data;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database;

public class ReadDbContext : DbContext, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {

    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
    }
}
