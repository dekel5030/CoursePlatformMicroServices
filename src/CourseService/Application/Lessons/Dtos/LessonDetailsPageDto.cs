using Courses.Application.Abstractions.Hateoas;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Lessons.Dtos;

public record LessonDetailsPageDto(
    LessonId LessonId,
    ModuleId ModuleId,
    CourseId CourseId,
    string CourseName,
    Title Title,
    Description Description,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    string Access,
    string? VideoUrl,
    IReadOnlyCollection<LinkDto> Links
);
