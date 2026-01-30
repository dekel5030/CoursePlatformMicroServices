using Courses.Application.Abstractions.Data.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Abstractions.Data;

public interface IReadDbContext
{
    // Core Read Models (new architecture)
    DbSet<CourseReadModel> Courses { get; }
    DbSet<ModuleReadModel> Modules { get; }
    DbSet<LessonReadModel> Lessons { get; }
    DbSet<InstructorReadModel> Instructors { get; }
    DbSet<CategoryReadModel> Categories { get; }

    // Legacy Read Models (to be removed after migration)
    DbSet<CourseHeaderReadModel> CourseHeaders { get; }
    DbSet<CourseStructureReadModel> CourseStructures { get; }
    DbSet<CourseStatsReadModel> CourseStats { get; }
    DbSet<CourseListItemReadModel> CourseListItems { get; }
    DbSet<LessonDetailsReadModel> LessonDetails { get; }
    DbSet<ModuleDetailsReadModel> ModuleDetails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
