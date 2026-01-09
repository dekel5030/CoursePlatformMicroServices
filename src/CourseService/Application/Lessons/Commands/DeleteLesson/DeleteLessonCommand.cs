using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.DeleteLesson;

public record DeleteLessonCommand(CourseId CourseId, LessonId LessonId) : ICommand;