using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.ProcessLesson;

public sealed record ProcessLessonCommand(LessonId LessonId, string Message) : ICommand;
