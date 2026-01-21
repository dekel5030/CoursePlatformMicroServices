using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record CourseSummaryDto(
    CourseId Id,
    Title Title,
    InstructorDto Instructor,
    CourseStatus Status,
    Money Price,
    string? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount,
    DateTimeOffset UpdatedAtUtc
);
