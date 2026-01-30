using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.GenerateLessonWithAi;

public sealed record GenerateLessonWithAiCommand(
    ModuleId ModuleId,
    LessonId LessonId) : ICommand<GenerateLessonWithAiResponse>;
