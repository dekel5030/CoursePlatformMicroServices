namespace Courses.Application.Modules.Dtos;

public sealed record ManagedModuleDto(
    ModuleDto Module,
    ManagedModuleStatsDto Stats
);

public sealed record ManagedModuleStatsDto(
    int LessonCount,
    TimeSpan Duration
);
