using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Lessons.Dtos;

public sealed record LessonDetailsPageDto
{
    public required Guid LessonId { get; init; }
    public required Guid ModuleId { get; init; }
    public required Guid CourseId { get; init; }
    public required string CourseName { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int Index { get; init; }
    public required TimeSpan Duration { get; init; }
    public required LessonAccess Access { get; init; }
    public required string? ThumbnailUrl { get; init; }
    public required string? VideoUrl { get; init; }
    public required string? TranscriptUrl { get; init; }
    public required IReadOnlyList<LinkDto> Links { get; init; }
}
