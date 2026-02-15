using Courses.Application.Categories.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Application.Users.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;

namespace Courses.Application.Features.CoursePage;

public record CoursePageDto
{
    public required CoursePageCourseDto Course { get; init; }
    public required CourseAnalyticsDto Analytics { get; init; }
    public required CourseStructureDto Structure { get; init; }

    public required IReadOnlyDictionary<Guid, CoursePageModuleDto> Modules { get; init; }
    public required IReadOnlyDictionary<Guid, CoursePageLessonDto> Lessons { get; init; }
    public required IReadOnlyDictionary<Guid, UserDto> Instructors { get; init; }
    public required IReadOnlyDictionary<Guid, CategoryDto> Categories { get; init; }
}

public sealed record CoursePageCourseDto(CoursePageCourseData Data, CoursePageCourseLinks Links);

public sealed record CoursePageCourseData(
    Guid Id,
    string Title,
    string Description,
    Money Price,
    CourseStatus Status,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<string> Tags,
    Guid InstructorId,
    Guid CategoryId);

public sealed record CoursePageCourseLinks(
    LinkRecord Self,
    LinkRecord? Manage,
    LinkRecord Ratings);

public sealed record CoursePageModuleDto(CoursePageModuleData Data, CoursePageModuleLinks Links);

public sealed record CoursePageModuleData(
    Guid Id,
    string Title,
    int LessonCount,
    TimeSpan TotalDuration);

public sealed record CoursePageModuleLinks();

public sealed record CoursePageLessonDto(CoursePageLessonData Data, CoursePageLessonLinks Links);

public sealed record CoursePageLessonData(
    Guid Id,
    string Title,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    LessonAccess Access,
    Guid ModuleId,
    Guid CourseId,
    string Description,
    string? VideoUrl,
    string? TranscriptUrl);

public sealed record CoursePageLessonLinks(LinkRecord? Self);
