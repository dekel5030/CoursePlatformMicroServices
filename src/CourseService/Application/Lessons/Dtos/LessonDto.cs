using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Lessons.Dtos;

public record LessonDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required int Index { get; init; }
    public required TimeSpan Duration { get; init; }
    public required string? ThumbnailUrl { get; init; }
    public required LessonAccess Access { get; init; }
    public Guid? ModuleId { get; init; }
    public Guid? CourseId { get; init; }
    public string? Description { get; init; }
    public string? VideoUrl { get; init; }
    public string? TranscriptUrl { get; init; }
    public required List<LinkDto> Links { get; init; }
}
