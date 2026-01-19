using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
