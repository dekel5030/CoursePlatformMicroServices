using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public record CreateLessonCommand(
    CourseId CourseId,
    string? Title,
    string? Description) : ICommand<LessonDetailsDto>;
