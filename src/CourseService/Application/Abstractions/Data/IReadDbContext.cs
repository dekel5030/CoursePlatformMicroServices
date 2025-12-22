using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }
}
