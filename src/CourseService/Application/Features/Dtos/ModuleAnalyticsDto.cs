namespace Courses.Application.Features.Dtos;

public sealed record ModuleAnalyticsDto(
    int LessonCount,
    TimeSpan Duration
);
