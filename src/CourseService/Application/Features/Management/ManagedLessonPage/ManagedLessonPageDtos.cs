using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Features.Management.ManagedLessonPage;

public sealed record ManagedLessonPageDto(
    ManagedLessonPageData Data,
    ManagedLessonPageLinks Links);

public sealed record ManagedLessonPageData(
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

public sealed record ManagedLessonPageLinks(
    LinkRecord Self,
    LinkRecord? ManagedCourse,
    LinkRecord? PublicPreview,
    LinkRecord? PartialUpdate,
    LinkRecord? Delete,
    LinkRecord? GenerateVideoUploadUrl,
    LinkRecord? AiGenerate,
    LinkRecord? ManageTranscript,
    LinkRecord? NextLesson,
    LinkRecord? PreviousLesson
);
