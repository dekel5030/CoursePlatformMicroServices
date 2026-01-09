using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Courses.Queries.Dtos;

public record CourseSummaryDto(
    CourseId Id,
    string Title,
    string? InstructorName,
    decimal Price,
    string Currency,
    string? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount
);