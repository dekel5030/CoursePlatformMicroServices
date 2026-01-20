using Courses.Application.Actions.Primitives;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Courses.Domain.Users;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record CourseDetailsDto(
    CourseId Id,
    Title Title,
    Description Description,
    InstructorDto Instructor,
    CourseStatus Status,
    Money Price,
    int EnrollmentCount,
    int LessonsCount,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<LessonSummaryDto> Lessons
);
