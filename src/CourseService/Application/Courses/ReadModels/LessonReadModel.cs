using Courses.Application.Courses.Dtos;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Courses.ReadModels;

public record LessonReadModel
{
    public required Guid LessonId { get; init; }
    public required string Title { get; init; }
    public required int Index { get; init; }
    public required TimeSpan Duration { get; init; }
    public required string? ThumbnailUrl { get; init; }
    public required LessonAccess Access { get; init; }

    public LessonDto ToDto()
    {
        return new LessonDto
        {
            Id = LessonId,
            Title = Title,
            Index = Index,
            Duration = Duration,
            ThumbnailUrl = ThumbnailUrl,
            Access = Access,
            Links = []
        };
    }
}
