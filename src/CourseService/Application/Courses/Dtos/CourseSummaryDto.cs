using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Courses.Dtos;

public record CourseSummaryDto(
    CourseId Id,
    Title Title,
    string? InstructorName,
    decimal Price,
    string Currency,
    string? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount
);
