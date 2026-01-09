using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.PatchLesson;

public record PatchLessonCommand(
    Guid CourseId,
    Guid LessonId,
    string? Title,
    string? Description,
    LessonAccess? Access) : ICommand;
