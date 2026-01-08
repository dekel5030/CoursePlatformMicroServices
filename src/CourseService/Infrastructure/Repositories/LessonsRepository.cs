using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Repositories;

public class LessonsRepository : IRepository<Lesson, LessonId>
{
    private readonly WriteDbContext _dbContext;

    public LessonsRepository(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Lesson entity)
    {
        _dbContext.Lessons.Add(entity);
    }

    public Task<Lesson?> GetByidAsync(
        LessonId id, 
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Lessons
            .FirstOrDefaultAsync(lesson => lesson.Id == id, cancellationToken);
    }
}