using System;
using System.Collections.Generic;
using System.Text;

namespace CoursePlatform.Contracts.CourseService;

public record LessonCreatedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    string Slug,
    string Title,
    string Description,
    string Access,
    TimeSpan Duration,
    int Index,
    string? VideoUrl,
    string? ThumbnailUrl,
    string? TranscriptUrl);

public record LessonMetadataChangedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    string Title,
    string Description,
    string Slug);

public record LessonMediaChangedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    string? VideoUrl,
    string? ThumbnailUrl,
    TimeSpan Duration);

public record LessonAccessChangedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    string NewAccess);

public record LessonIndexChangedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    int NewIndex);

public record LessonTranscriptChangedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    string? TranscriptUrl);

public record LessonDeletedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId);