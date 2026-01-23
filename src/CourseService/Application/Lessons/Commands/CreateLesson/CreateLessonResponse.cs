using Courses.Domain.Courses.Primitives;
using Courses.Domain.Module.Primitives;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public sealed record CreateLessonResponse(CourseId CourseId, ModuleId ModuleId);
