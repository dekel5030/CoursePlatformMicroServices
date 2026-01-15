using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Courses.Queries.Dtos;

public record CourseSummaryDto(
    CourseId Id,
    Title Title,
    string? InstructorName,
    decimal Price,
    string Currency,
    ImageUrl? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount,
    IReadOnlyList<CourseAction> AllowedActions
);