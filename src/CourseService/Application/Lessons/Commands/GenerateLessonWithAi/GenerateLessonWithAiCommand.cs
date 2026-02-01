using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.GenerateLessonWithAi;

public sealed record GenerateLessonWithAiCommand(LessonId LessonId) : ICommand<GenerateLessonWithAiResponse>;
