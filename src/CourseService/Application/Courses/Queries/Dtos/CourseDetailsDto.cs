using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Courses.Queries.Dtos;

public record CourseDetailsDto(
    CourseId Id,
    string Title,
    string Description,
    string? InstructorName,
    decimal Price,
    string Currency,
    int EnrollmentCount,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<LessonSummaryDto> Lessons
);