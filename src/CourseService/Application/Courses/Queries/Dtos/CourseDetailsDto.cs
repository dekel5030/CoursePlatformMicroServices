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
    public string Value { get; }
    private CourseAction(string value) => Value = value;

    public static readonly CourseAction Edit = new("Edit");
    public static readonly CourseAction Publish = new("Publish");
    public static readonly CourseAction CreateLesson = new("CreateLesson");
    public static readonly CourseAction Delete = new("Delete");
}

public sealed record CourseCollectionAction
{
    public string Value { get; }
    private CourseCollectionAction(string value) => Value = value;

    public static readonly CourseCollectionAction CreateCourse = new("CreateCourse");
}