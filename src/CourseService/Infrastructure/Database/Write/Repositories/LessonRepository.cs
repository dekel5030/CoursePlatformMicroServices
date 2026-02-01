using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Infrastructure.Database.Write.Repositories;

public class LessonRepository : RepositoryBase<Lesson, LessonId>, ILessonRepository
{
    public LessonRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}
