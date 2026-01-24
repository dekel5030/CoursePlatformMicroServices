using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Lessons.Dtos;

public record LessonDetailsPageDto(
    Guid LessonId,
    Guid ModuleId,
    Guid CourseId,
    string CourseName,
    string Title,
    string Description,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    string Access,
    string? VideoUrl,
    IReadOnlyCollection<LinkDto> Links
);
