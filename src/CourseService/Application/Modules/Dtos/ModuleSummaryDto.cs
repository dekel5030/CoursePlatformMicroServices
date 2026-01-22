using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Modules.Dtos;

public record ModuleSummaryDto(
    ModuleId Id,
    Title Title,
    int Index,
    int LessonCount
);
