using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.PatchCourse;

public record PatchCourseCommand(
    CourseId CourseId,
    Title? Title,
    Description? Description,
    Guid? InstructorId,
    decimal? PriceAmount,
    string? PriceCurrency) : ICommand;
