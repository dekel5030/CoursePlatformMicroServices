using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.CourseViews;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Ratings;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<Module> Modules { get; }
    DbSet<Category> Categories { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<CourseRating> CourseRatings { get; }
    DbSet<User> Users { get; }
    DbSet<CourseView> CourseViews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
