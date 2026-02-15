using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Features.LessonPage;

public sealed record LessonPageDto(LessonPageData Data, LessonPageLinks Links);

public sealed record LessonPageData(
    Guid LessonId,
    Guid ModuleId,
    Guid CourseId,
    string CourseName,
    string Title,
    string Description,
    int Index,
    TimeSpan Duration,
    LessonAccess Access,
    string? ThumbnailUrl,
    string? VideoUrl,
    string? TranscriptUrl);

public sealed record LessonPageLinks(
    LinkRecord Self,
    LinkRecord Course);
