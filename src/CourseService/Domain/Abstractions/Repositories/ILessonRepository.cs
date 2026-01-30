using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Domain.Abstractions.Repositories;

public interface ILessonRepository : IRepository<Lesson, LessonId>;
