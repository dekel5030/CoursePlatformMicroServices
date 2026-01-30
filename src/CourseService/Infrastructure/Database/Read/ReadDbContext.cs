using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database.Read;

public class ReadDbContext : DbContext, IReadDbContext
{
    // Core Read Models (new architecture)
    public DbSet<CourseReadModel> Courses { get; set; }
    public DbSet<ModuleReadModel> Modules { get; set; }
    public DbSet<LessonReadModel> Lessons { get; set; }
    public DbSet<InstructorReadModel> Instructors { get; set; }
    public DbSet<CategoryReadModel> Categories { get; set; }

    // Legacy Read Models (to be removed after migration)
    public DbSet<CourseHeaderReadModel> CourseHeaders { get; set; }
    public DbSet<CourseStructureReadModel> CourseStructures { get; set; }
    public DbSet<CourseStatsReadModel> CourseStats { get; set; }
    public DbSet<CourseListItemReadModel> CourseListItems { get; set; }
    public DbSet<LessonDetailsReadModel> LessonDetails { get; set; }
    public DbSet<ModuleDetailsReadModel> ModuleDetails { get; set; }

    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DependencyInjection).Assembly,
            type => type.Namespace is not null &&
                type.Namespace.StartsWith(
                    typeof(ReadDbContext).Namespace!,
                    StringComparison.InvariantCultureIgnoreCase));

        modelBuilder.HasDefaultSchema(SchemaNames.Read);
    }
}
