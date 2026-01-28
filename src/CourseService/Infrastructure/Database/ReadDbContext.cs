using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.ReadModels;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Module;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;

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

        modelBuilder.Entity<CoursePageReadModel>(builder =>
        {
            builder.ToTable("course_pages");
            builder.HasKey(x => x.Id);

            builder.OwnsMany(x => x.Modules, m =>
            {
                m.ToJson();
                m.OwnsMany(module => module.Lessons);
            });
        });

    }
}
