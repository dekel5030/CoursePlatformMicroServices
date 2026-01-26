using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.UpdateLessonWithAi;

public sealed record UpdateLessonWithAiCommand(
    ModuleId ModuleId,
    LessonId LessonId) : ICommand<UpdateLessonAiResponse>;
