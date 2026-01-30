using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public sealed record CreateLessonCommand(
    ModuleId ModuleId,
    Title? Title,
    Description? Description) : ICommand<CreateLessonResponse>;
