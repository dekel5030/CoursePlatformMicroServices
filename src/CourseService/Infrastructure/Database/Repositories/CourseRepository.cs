using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database.Repositories;

public class CourseRepository : RepositoryBase<Course, CourseId>, ICourseRepository
{
    public CourseRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<Course?> GetByIdAsync(
        CourseId id,
        CancellationToken cancellationToken = default)
    {
        return DbContext.Courses
            .Include(course => course.Lessons)
            .FirstOrDefaultAsync(course => course.Id == id, cancellationToken);
    }
}