using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record CourseSummaryDto(
    Guid Id,
    string Title,
    InstructorDto Instructor,
    CourseStatus Status,
    Money Price,
    string? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount,
    DateTimeOffset UpdatedAtUtc
);
