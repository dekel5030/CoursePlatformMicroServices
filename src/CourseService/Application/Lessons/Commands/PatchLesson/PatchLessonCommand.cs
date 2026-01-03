using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.PatchLesson;

public record PatchLessonCommand(
    Guid LessonId,
    string? Title,
    string? Description,
    string? Access,
    string? ThumbnailImageUrl,
    string? VideoUrl,
    TimeSpan? Duration)
        : ICommand;
