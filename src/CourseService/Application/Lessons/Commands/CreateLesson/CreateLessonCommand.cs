using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.CreateLesson;

public record CreateLessonCommand(
    CourseId CourseId,
    Title? Title,
    Description? Description) : ICommand<LessonDetailsDto>;
