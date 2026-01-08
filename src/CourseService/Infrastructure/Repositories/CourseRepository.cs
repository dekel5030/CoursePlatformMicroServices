using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Repositories;

public class CourseRepository : IRepository<Course, CourseId>
{
    private readonly WriteDbContext _dbContext;

    public CourseRepository(WriteDbContext writeDbContext)
    {
        _dbContext = writeDbContext;
    }

    public void Add(Course entity)
    {
        _dbContext.Courses.Add(entity);
    }

    public Task<Course?> GetByidAsync(
        CourseId id, 
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Courses
            .Include(course => course.Lessons)
            .FirstOrDefaultAsync(course => course.Id == id, cancellationToken);
    }
}
