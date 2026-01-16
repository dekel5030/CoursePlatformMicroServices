using Courses.Application.Actions.Primitives;
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
    IReadOnlyCollection<CourseAction> AllowedActions
);
