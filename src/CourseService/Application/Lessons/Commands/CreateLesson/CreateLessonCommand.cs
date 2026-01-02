using Courses.Application.Courses.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public record CreateLessonCommand(
    Guid CourseId,
    string? Title,
    string? Description) : ICommand<LessonDetailsDto>;
