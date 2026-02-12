using Courses.Application.Modules.Dtos;

namespace Courses.Application.Features.Management;

public sealed record ManagedModuleDto(
    ModuleDto Module,
    ManagedModuleStatsDto Stats
);

public sealed record ManagedModuleStatsDto(
    int LessonCount,
    TimeSpan Duration
);
