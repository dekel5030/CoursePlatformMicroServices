using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Module;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<User> Users { get; }
    DbSet<Module> Modules { get; }
    DbSet<Category> Categories { get; }
}
