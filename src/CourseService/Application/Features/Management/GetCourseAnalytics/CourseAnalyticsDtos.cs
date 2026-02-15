namespace Courses.Application.Features.Management.GetCourseAnalytics;


public sealed record CourseDetailedAnalyticsDto
{
    public required int EnrollmentsCount { get; init; }
    public required double AverageRating { get; init; }
    public required int ReviewsCount { get; init; }
    public required int ViewCount { get; init; }
    public required int TotalLessonsCount { get; init; }
    public required TimeSpan TotalCourseDuration { get; init; }

    public required IReadOnlyList<ModuleAnalyticsSummaryDto> ModuleAnalytics { get; init; }

    public required IReadOnlyList<EnrollmentCountByDayDto> EnrollmentsOverTime { get; init; }


    public required IReadOnlyList<CourseViewerDto> CourseViewers { get; init; }
}


public sealed record CourseViewerDto(
    Guid UserId,
    string DisplayName,
    string? AvatarUrl,
    DateTimeOffset ViewedAt);

public sealed record ModuleAnalyticsSummaryDto(
    Guid ModuleId,
    int LessonCount,
    TimeSpan TotalDuration);

public sealed record EnrollmentCountByDayDto(DateTimeOffset Date, int Count);
