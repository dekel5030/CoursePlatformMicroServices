using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record CourseSummaryDto(
    CourseId Id,
    Title Title,
    UserId InstructorId,
    string? InstructorName,
    CourseStatus Status,
    Money Price,
    string? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount,
    DateTimeOffset UpdatedAtUtc
);
