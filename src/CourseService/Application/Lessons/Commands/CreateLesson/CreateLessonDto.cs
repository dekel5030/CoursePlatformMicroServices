namespace Courses.Application.Lessons.Commands.CreateLesson;

public record CreateLessonDto(
    string? Title,
    string? Description,
    string? VideoUrl,
    string? ThumbnailImage,
    bool IsPreview,
    int Order,
    TimeSpan? Duration,
    Guid CourseId
);
