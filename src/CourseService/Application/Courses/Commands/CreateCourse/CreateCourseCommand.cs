using Courses.Application.Courses.Dtos;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourse;

public record CreateCourseCommand(
    Title? Title,
    Description? Description) : ICommand<CourseSummaryDto>;
