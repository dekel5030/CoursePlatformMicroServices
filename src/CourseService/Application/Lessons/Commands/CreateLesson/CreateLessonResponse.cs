using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public sealed record CreateLessonResponse(CourseId CourseId, LessonId LessonId, Title Title);
