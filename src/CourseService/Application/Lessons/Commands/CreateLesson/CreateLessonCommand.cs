using Application.Abstractions.Messaging;
using Domain.Lessons.Primitives;
using SharedKernel;

namespace Application.Lessons.Commands.CreateLesson;

public record CreateLessonCommand(CreateLessonDto Dto) : ICommand<LessonId>;
