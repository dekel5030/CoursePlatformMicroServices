using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.DeleteLesson;

public record DeleteLessonCommand(ModuleId ModuleId, LessonId LessonId) : ICommand;
