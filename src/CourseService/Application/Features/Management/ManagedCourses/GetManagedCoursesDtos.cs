using Courses.Application.Categories.Dtos;
using Courses.Application.Features.Management;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Application.Users.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Features.Management.ManagedCourses;

public sealed record GetManagedCoursesDto(
    IReadOnlyList<ManagedCourseSummaryItemDto> Items,
    int PageNumber,
    int PageSize,
    int TotalItems,
    GetManagedCoursesCollectionLinks Links);

public sealed record ManagedCourseSummaryItemDto(
    ManagedCourseSummaryData Data,
    ManagedCourseSummaryLinks Links);

public sealed record ManagedCourseSummaryData(
    Guid Id,
    string Title,
    string ShortDescription,
    string Slug,
    UserDto Instructor,
    CategoryDto Category,
    Money Price,
    DifficultyLevel Difficulty,
    string? ThumbnailUrl,
    DateTimeOffset UpdatedAtUtc,
    CourseStatus Status,
    ManagedCourseStatsDto Stats);

public sealed record ManagedCourseSummaryLinks(
    LinkRecord Self,
    LinkRecord? CoursePage);

public sealed record GetManagedCoursesCollectionLinks(LinkRecord Self);
