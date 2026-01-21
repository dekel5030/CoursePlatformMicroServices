using Courses.Application.Lessons.Dtos;
using Courses.Domain.Module.Primitives;

namespace Courses.Application.Modules.Dtos;

public record ModuleDetailsDto(
    ModuleId Id,
    string Title,
    int Index,
    IReadOnlyList<LessonSummaryDto> Lessons
);
