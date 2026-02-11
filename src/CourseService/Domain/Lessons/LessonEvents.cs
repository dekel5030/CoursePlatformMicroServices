using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Lessons;

public record LessonCreatedDomainEvent(
    LessonId Id,
    ModuleId ModuleId,
    CourseId CourseId,
    Slug Slug,
    Title Title,
    Description Description,
    LessonAccess Access,
    TimeSpan Duration,
    int Index,
    VideoUrl? VideoUrl,
    ImageUrl? ThumbnailUrl,
    Url? TranscriptUrl) : IDomainEvent;

public record LessonMetadataChangedDomainEvent(
    LessonId Id,
    ModuleId ModuleId,
    CourseId CourseId,
    Title Title,
    Description Description,
    Slug Slug) : IDomainEvent;

public record LessonMediaChangedDomainEvent(
    LessonId Id,
    ModuleId ModuleId,
    CourseId CourseId,
    VideoUrl? VideoUrl,
    ImageUrl? ThumbnailUrl,
    TimeSpan Duration) : IDomainEvent;

public record LessonAccessChangedDomainEvent(
    LessonId Id,
    ModuleId ModuleId,
    CourseId CourseId,
    LessonAccess NewAccess) : IDomainEvent;

public record LessonIndexChangedDomainEvent(
    LessonId Id,
    ModuleId ModuleId,
    CourseId CourseId,
    int NewIndex) : IDomainEvent;

public record LessonTranscriptChangedDomainEvent(
    LessonId Id,
    ModuleId ModuleId,
    CourseId CourseId,
    Url? TranscriptUrl) : IDomainEvent;

public record LessonDeletedDomainEvent(
    LessonId Id,
    ModuleId ModuleId,
    CourseId CourseId) : IDomainEvent;

public record LessonMovedDomainEvent(
    LessonId Id,
    ModuleId PreviousModuleId,
    ModuleId NewModuleId,
    CourseId CourseId,
    int NewIndex) : IDomainEvent;