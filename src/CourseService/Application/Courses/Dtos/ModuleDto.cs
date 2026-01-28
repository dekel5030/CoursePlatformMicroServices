using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Courses.Dtos;

public record ModuleDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required int Index { get; init; }
    public required int LessonCount { get; init; }
    public required TimeSpan Duration { get; init; }
    public required IReadOnlyList<LessonDto> Lessons { get; init; }
    public required List<LinkDto> Links { get; init; }
};
