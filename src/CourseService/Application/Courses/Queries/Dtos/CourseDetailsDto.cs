using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Courses.Queries.Dtos;

public record CourseDetailsDto(
    CourseId Id,
    Title Title,
    Description Description,
    string? InstructorName,
    decimal Price,
    string Currency,
    int EnrollmentCount,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<ImageUrl> ImageUrls,
    IReadOnlyList<LessonSummaryDto> Lessons,
    IReadOnlyList<CourseAction> AllowedActions
);

public sealed record CourseAction
{
    public string Value { get; set; }
    private CourseAction(string value) => Value = value;

    public static CourseAction Edit => new("Edit");
    public static CourseAction Delete => new("Delete");
}