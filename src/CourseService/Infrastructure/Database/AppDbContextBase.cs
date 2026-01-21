using Courses.Application.Abstractions.Data;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Module;
using Courses.Domain.Users;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database;

public abstract class AppDbContextBase : DbContext, IUnitOfWork
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Module> Modules { get; set; }

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
