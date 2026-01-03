using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.PatchCourse;

public record PatchCourseCommand(
    Guid CourseId,
    string? Title,
    string? Description,
    Guid? InstructorId,
    decimal? PriceAmount,
    string? PriceCurrency)
        : ICommand;
