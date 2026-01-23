using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Application.Courses.Queries.GetById;

public record LessonDto(
    Guid LessonId,
    string Title,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    string Access,
    IReadOnlyList<LinkDto> Links
);
