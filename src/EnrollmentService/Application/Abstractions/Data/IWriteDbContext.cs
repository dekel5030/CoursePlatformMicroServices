using Domain.Enrollments;
using Domain.Users;
using Domain.Courses;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IWriteDbContext
{
    DbSet<Enrollment> Enrollments { get; }
    DbSet<KnownUser> KnownUsers { get; }
    DbSet<KnownCourse> KnownCourses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
