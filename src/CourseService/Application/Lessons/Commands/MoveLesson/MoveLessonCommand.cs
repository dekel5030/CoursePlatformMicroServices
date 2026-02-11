using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.MoveLesson;

public sealed record MoveLessonCommand(
    LessonId LessonId,
    ModuleId TargetModuleId,
    int TargetIndex) : ICommand;
