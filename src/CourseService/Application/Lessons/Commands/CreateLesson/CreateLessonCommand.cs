using Application.Abstractions.Messaging;
using Domain.Lessons.Primitives;

namespace Application.Lessons.Commands.CreateLesson;

public record CreateLessonCommand(CreateLessonDto Dto) : ICommand<LessonId>;
