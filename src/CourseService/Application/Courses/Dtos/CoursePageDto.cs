using Courses.Application.Categories.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record CoursePageDto(
    CourseId Id,
    Title Title,
    Description Description,
    InstructorDto Instructor,
    CourseStatus Status,
    Money Price,
    int EnrollmentCount,
    int LessonsCount,
    TimeSpan TotalDuration,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<TagDto> Tags,
    CategoryDto Category,
    IReadOnlyList<ModuleDetailsDto> Modules
);
