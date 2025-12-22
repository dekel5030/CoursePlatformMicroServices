using Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Application.Lessons.Commands.CreateLesson;

public record CreateLessonCommand(CreateLessonDto Dto) : ICommand<LessonId>;
