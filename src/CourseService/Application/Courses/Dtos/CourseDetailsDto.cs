using Courses.Application.Lessons.Dtos;
using Courses.Application.Users;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record CourseDetailsDto(
    Guid Id,
    string Title,
    string Description,
    InstructorDto Instructor,
    CourseStatus Status,
    Money Price,
    int EnrollmentCount,
    int LessonsCount,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<LessonSummaryDto> Lessons
);
