using System.Text.Json.Serialization;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Courses.Dtos;

public record LessonDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required int Index { get; init; }
    public required TimeSpan Duration { get; init; }
    public required string? ThumbnailUrl { get; init; }
    public required LessonAccess Access { get; init; }
    public required List<LinkDto> Links { get; init; }
};
