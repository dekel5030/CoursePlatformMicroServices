using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Users;

namespace Courses.Application.Abstractions.Data;

public interface IReadDbContext
{
    IQueryable<Course> Courses { get; }
    IQueryable<Module> Modules { get; }
    IQueryable<Lesson> Lessons { get; }
    IQueryable<User> Users { get; }
    IQueryable<Category> Categories { get; }
    IQueryable<Enrollment> Enrollments { get; }
}
