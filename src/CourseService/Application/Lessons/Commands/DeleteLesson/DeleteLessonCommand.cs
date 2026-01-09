using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.DeleteLesson;

public record DeleteLessonCommand(Guid CourseId, Guid LessonId) : ICommand;