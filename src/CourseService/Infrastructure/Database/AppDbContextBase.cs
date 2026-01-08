using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database;

public abstract class AppDbContextBase : DbContext
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }

    protected AppDbContextBase(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
        modelBuilder.AddTransactionalOutboxEntities();
    }
}
