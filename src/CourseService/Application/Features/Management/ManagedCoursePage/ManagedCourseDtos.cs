using Courses.Application.Categories.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;

namespace Courses.Application.Features.Management.ManagedCoursePage;

public record ManagedCoursePageDto
{
    public required ManagedCoursePageCourseDto Course { get; init; }
    public required CourseStructureDto Structure { get; init; }

    public required IReadOnlyDictionary<Guid, ManagedCoursePageModuleDto> Modules { get; init; }
    public required IReadOnlyDictionary<Guid, ManagedCoursePageLessonDto> Lessons { get; init; }
    public required IReadOnlyDictionary<Guid, CategoryDto> Categories { get; init; }
}

public sealed record ManagedCoursePageCourseDto(CourseCoreDto Data, ManagedCourseLinks Links);
public sealed record ManagedCoursePageLessonDto(LessonCoreDto Data, ManagedLessonLinks Links);
public sealed record ManagedCoursePageModuleDto(ModuleCoreDto Data, ManagedModuleLinks Links);

public sealed record CourseCoreDto(
    Guid Id,
    string Title,
    string Description,
    CourseStatus Status,
    Money Price,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<string> Tags,
    Guid InstructorId,
    Guid CategoryId);

public sealed record LessonCoreDto(
    Guid Id,
    string Title,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    LessonAccess Access,
    Guid ModuleId,
    Guid CourseId,
    string? Description,
    string? VideoUrl,
    string? TranscriptUrl);

public sealed record ModuleCoreDto(
    Guid Id,
    string Title,
    int LessonCount,
    TimeSpan Duration);

public sealed record ManagedCourseLinks(
    LinkRecord Self,
    LinkRecord? CoursePage,
    LinkRecord? Analytics,
    LinkRecord? PartialUpdate,
    LinkRecord? Delete,
    LinkRecord? Publish,
    LinkRecord? GenerateImageUploadUrl,
    LinkRecord? CreateModule,
    LinkRecord? ChangePosition);

public sealed record ManagedLessonLinks(
    LinkRecord Self,
    LinkRecord? Manage,
    LinkRecord? PartialUpdate,
    LinkRecord? ChangePosition);

public sealed record ManagedModuleLinks(
    LinkRecord? CreateLesson,
    LinkRecord? PartialUpdate,
    LinkRecord? Delete,
    LinkRecord? ChangePosition);
