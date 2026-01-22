using Courses.Application.Lessons.Dtos;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Modules.Dtos;

public record ModuleDetailsDto(
    ModuleId Id,
    Title Title,
    int Index,
    int LessonCount,
    TimeSpan TotalDuration,
    IReadOnlyList<LessonSummaryDto> Lessons
);
