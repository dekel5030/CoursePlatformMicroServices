using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Module;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<CoursePage> CoursePages { get; }
    DbSet<CourseHeaderReadModel> CourseHeaders { get; }
    DbSet<CourseStructureReadModel> CourseStructures { get; }
    DbSet<CourseStatsReadModel> CourseStats { get; }
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<User> Users { get; }
    DbSet<Module> Modules { get; }
    DbSet<Category> Categories { get; }
    DbSet<Enrollment> Enrollments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
