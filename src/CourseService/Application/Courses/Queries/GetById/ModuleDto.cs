using Courses.Application.Abstractions.Hateoas;

namespace Courses.Application.Courses.Queries.GetById;

public record ModuleDto(
    Guid Id,
    string Title,
    int Index,
    int LessonCount,
    TimeSpan Duration,
    IReadOnlyList<LessonDto> Lessons,
    IReadOnlyList<LinkDto> Links
);
