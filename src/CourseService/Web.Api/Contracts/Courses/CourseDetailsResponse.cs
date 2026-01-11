using Courses.Api.Contracts.Lessons;

namespace Courses.Api.Contracts.Courses;

public record CourseDetailsResponse(
    Guid Id,
    string Title,
    string Description,
    string? InstructorName,
    decimal Price,
    string Currency,
    int EnrollmentCount,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<LessonSummaryResponse> Lessons);
