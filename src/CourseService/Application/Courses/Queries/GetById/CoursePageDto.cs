using Courses.Application.Abstractions.Hateoas;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.Queries.GetById;

public record CoursePageDto(
    Guid Id,
    string Title,
    string Description,
    Guid InstructorId,
    string InstructorName,
    string? InstructorAvatarUrl,
    CourseStatus Status,
    Money Price,
    int EnrollmentCount,
    int LessonsCount,
    TimeSpan TotalDuration,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<string> Tags,
    Guid CategoryId,
    string CategoryName,
    string CategorySlug,
    IReadOnlyList<ModuleDto> Modules,
    IReadOnlyList<LinkDto> Links
);
