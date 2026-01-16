using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Lessons.Dtos;

public record LessonDetailsDto(
    CourseId CourseId,
    LessonId LessonId,
    Title Title,
    Description Description,
    int Index,
    TimeSpan? Duration,
    bool IsPreview,
    string? ThumbnailUrl,
    string? VideoUrl,
    IReadOnlyCollection<LessonAction> AllowedActions
);
