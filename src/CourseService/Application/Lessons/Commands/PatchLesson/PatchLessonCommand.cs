using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.PatchLesson;

public record PatchLessonCommand(
    ModuleId ModuleId,
    LessonId LessonId,
    Title? Title,
    Description? Description,
    LessonAccess? Access) : ICommand;
