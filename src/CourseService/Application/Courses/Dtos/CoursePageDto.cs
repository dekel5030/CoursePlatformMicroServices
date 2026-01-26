using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record ModuleLessonDto(
    Guid LessonId,
    string Title,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    string Access,
    IReadOnlyList<LinkDto> Links
);

public record ModuleDto(
    Guid Id,
    string Title,
    int Index,
    int LessonCount,
    TimeSpan Duration,
    IReadOnlyList<ModuleLessonDto> Lessons,
    IReadOnlyList<LinkDto> Links
);

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
