using Courses.Application.Actions;
using Courses.Application.Lessons.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Lessons.Dtos;

public record LessonDetailsDto(
    CoursePolicyContext CourseContext,
    CourseId CourseId,
    ModuleId ModuleId,
    LessonId LessonId,
    Title Title,
    Description Description,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    LessonAccess Access,
    LessonStatus Status,
    string? VideoUrl
);
