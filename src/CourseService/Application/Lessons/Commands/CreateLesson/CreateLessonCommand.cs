using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public record CreateLessonCommand(CreateLessonDto Dto) : ICommand<LessonId>;
