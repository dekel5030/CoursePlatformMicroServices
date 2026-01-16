using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Lessons.Queries.Dtos;

public record LessonSummaryDto(
    CourseId CourseId,
    LessonId LessonId,
    Title Title,
    Description Description,
    int Index,
    TimeSpan? Duration,
    bool IsPreview,
    ImageUrl? ThumbnailUrl,
    IReadOnlyCollection<LessonAction> AllowedActions
);
