using Courses.Application.Lessons.Dtos;

namespace Courses.Application.Modules.Dtos;

public record ModuleDetailsDto(
    Guid Id,
    string Title,
    int Index,
    int LessonCount,
    TimeSpan TotalDuration,
    IReadOnlyList<LessonSummaryDto> Lessons
);
