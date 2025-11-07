using Domain.Courses;
using Domain.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
