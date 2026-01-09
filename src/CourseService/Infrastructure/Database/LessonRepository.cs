using Courses.Application.Abstractions.Data.Repositories;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Infrastructure.Database.Repositories;

namespace Courses.Infrastructure.Database;

internal class LessonRepository : RepositoryBase<Lesson, LessonId>, ILessonRepository
{
    public LessonRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}