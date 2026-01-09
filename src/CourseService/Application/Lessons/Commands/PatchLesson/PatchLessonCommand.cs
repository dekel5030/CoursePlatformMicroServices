using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.PatchLesson;

public record PatchLessonCommand(
    CourseId CourseId,
    LessonId LessonId,
    string? Title,
    string? Description,
    LessonAccess? Access) : ICommand;
