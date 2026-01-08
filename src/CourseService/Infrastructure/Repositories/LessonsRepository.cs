using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Infrastructure.Database;

namespace Courses.Infrastructure.Repositories;

public class LessonsRepository : RepositoryBase<Lesson, LessonId>
{
    public LessonsRepository(WriteDbContext dbContext) : base(dbContext) { }
}