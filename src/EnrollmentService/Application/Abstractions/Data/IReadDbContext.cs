using Domain.Courses;
using Domain.Enrollments;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<Enrollment> Enrollments { get; }
    DbSet<KnownUser> KnownUsers { get; }
    DbSet<KnownCourse> KnownCourses { get; }
}
