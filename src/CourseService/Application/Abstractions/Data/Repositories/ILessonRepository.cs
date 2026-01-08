using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Abstractions.Data.Repositories;

public interface ILessonRepository : IRepository<Lesson, LessonId>
{

}

