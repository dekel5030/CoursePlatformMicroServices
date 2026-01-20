using Courses.Application.Actions.Primitives;
using Courses.Application.Lessons.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Courses.Dtos;

public record CourseDetailsDto(
    CourseId Id,
    Title Title,
    Description Description,
    string? InstructorName,
    decimal Price,
    string Currency,
    int EnrollmentCount,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<LessonSummaryDto> Lessons,
    IReadOnlyCollection<CourseAction>? AllowedActions = null
);
