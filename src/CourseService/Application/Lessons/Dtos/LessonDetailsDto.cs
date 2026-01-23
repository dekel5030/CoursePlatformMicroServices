using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Lessons.Dtos;

public record LessonDetailsDto(
    ModuleId ModuleId,
    LessonId LessonId,
    Title Title,
    Description Description,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    LessonAccess Access,
    string? VideoUrl
);
