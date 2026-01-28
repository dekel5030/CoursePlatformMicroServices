using Courses.Application.Courses.Dtos;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Courses.ReadModels;

public sealed class LessonReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Index { get; set; }
    public TimeSpan Duration { get; set; }
    public string? ThumbnailUrl { get; set; }
    public LessonAccess Access { get; set; }

    public LessonDto ToDto()
    {
        return new LessonDto
        {
            Id = Id,
            Title = Title,
            Index = Index,
            Duration = Duration,
            ThumbnailUrl = ThumbnailUrl,
            Access = Access,
            Links = []
        };
    }
}
