using Courses.Application.Abstractions.Data.Repositories;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Infrastructure.Database;

namespace Courses.Infrastructure.Repositories;

public class LessonsRepository : RepositoryBase<Lesson, LessonId>, ILessonRepository
{
    public LessonsRepository(WriteDbContext dbContext) : base(dbContext) { }
}