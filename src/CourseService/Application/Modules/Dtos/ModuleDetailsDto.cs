using Courses.Application.Lessons.Dtos;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Modules.Dtos;

public record ModuleDetailsDto(
    Guid Id,
    string Title,
    int Index,
    int LessonCount,
    TimeSpan TotalDuration,
    IReadOnlyList<LessonSummaryDto> Lessons
);
