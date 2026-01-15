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
    IReadOnlyList<LessonAction> AllowedActions
);

public sealed record LessonAction
{
    public string Value { get; init; }

    private LessonAction(string value)
    {
        Value = value;
    }

    public static LessonAction Update => new("Update");
    public static LessonAction Delete => new("Delete");
    public static LessonAction Create => new("Create");
}