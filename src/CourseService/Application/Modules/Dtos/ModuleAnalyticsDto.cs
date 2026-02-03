namespace Courses.Application.Modules.Dtos;

public sealed record ModuleAnalyticsDto(
    int LessonCount,
    TimeSpan Duration
);
