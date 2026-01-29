using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Module;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database.Read;

public class ReadDbContext : AppDbContextBase, IReadDbContext
{
    public DbSet<CoursePage> CoursePages { get; set; }
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.ApplyConfigurationsFromAssembly(
        //    typeof(DependencyInjection).Assembly,
        //    type => type.Namespace is not null &&
        //        type.Namespace.StartsWith(
        //            typeof(ReadDbContext).Namespace!,
        //            StringComparison.InvariantCultureIgnoreCase));

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);

        modelBuilder.HasDefaultSchema(SchemaNames.Read);
        modelBuilder.Entity<Course>().ToTable("Courses", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Lesson>().ToTable("Lessons", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<User>().ToTable("Users", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Enrollment>().ToTable("Enrollments", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Category>().ToTable("Categories", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Module>().ToTable("Modules", SchemaNames.Write, t => t.ExcludeFromMigrations());
    }
}
