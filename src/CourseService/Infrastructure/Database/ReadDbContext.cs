using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.ReadModels;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Module;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Infrastructure.Database;

public class ReadDbContext : AppDbContextBase, IReadDbContext
{
    public DbSet<CoursePageReadModel> CoursePages { get; set; }
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Read);
        modelBuilder.Entity<Course>().ToTable("Courses", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Lesson>().ToTable("Lessons", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<User>().ToTable("Users", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Enrollment>().ToTable("Enrollments", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Category>().ToTable("Categories", SchemaNames.Write, t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Module>().ToTable("Modules", SchemaNames.Write, t => t.ExcludeFromMigrations());
    }
}

public class CoursePageConfiguration : IEntityTypeConfiguration<CoursePageReadModel>
{
    public void Configure(EntityTypeBuilder<CoursePageReadModel> builder)
    {
        builder.ToTable("course_pages", SchemaNames.Read);
        builder.HasKey(coursePage => coursePage.Id);

        builder.OwnsMany(cp => cp.Modules, moduleBuilder =>
        {
            moduleBuilder.ToJson();

            moduleBuilder.OwnsMany(m => m.Lessons, lessonBuilder => { });
        });
    }
}
