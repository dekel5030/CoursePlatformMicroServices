namespace Courses.Application.Courses.Queries.Dtos;

public record CourseSummaryDto(
    Guid Id,
    string Title,
    string? InstructorName,
    decimal Price,
    string Currency,
    string? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount
);