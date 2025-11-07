using Domain.Courses;
using Domain.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }
}
