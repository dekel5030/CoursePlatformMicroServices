using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.DeleteLesson;

public record DeleteLessonCommand(LessonId LessonId) : ICommand;
