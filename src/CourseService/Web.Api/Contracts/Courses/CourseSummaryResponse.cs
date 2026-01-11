namespace Courses.Api.Contracts.Courses;

public record CourseSummaryResponse(
    Guid Id,
    string Title,
    string? InstructorName,
    decimal Price,
    string Currency,
    string? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount);
