namespace Courses.Application.Courses.Dtos;

/// <summary>
/// Detailed analytics for the instructor's course analytics view (GET manage/courses/{id}/analytics).
/// </summary>
public sealed record CourseDetailedAnalyticsDto
{
    public required int EnrollmentsCount { get; init; }
    public required double AverageRating { get; init; }
    public required int ReviewsCount { get; init; }
    public required int ViewCount { get; init; }
    public required int TotalLessonsCount { get; init; }
    public required TimeSpan TotalCourseDuration { get; init; }

    /// <summary>
    /// Per-module analytics for engagement heat-map.
    /// </summary>
    public required IReadOnlyList<ModuleAnalyticsSummaryDto> ModuleAnalytics { get; init; }

    /// <summary>
    /// Time-series enrollment data (e.g. last 30 days).
    /// </summary>
    public required IReadOnlyList<EnrollmentCountByDayDto> EnrollmentsOverTime { get; init; }

    /// <summary>
    /// Users who viewed the course (name, avatar, last viewed), e.g. last 50.
    /// </summary>
    public required IReadOnlyList<CourseViewerDto> CourseViewers { get; init; }
}

/// <summary>
/// A user who viewed the course (for analytics viewer list).
/// </summary>
public sealed record CourseViewerDto(
    Guid UserId,
    string DisplayName,
    string? AvatarUrl,
    DateTimeOffset ViewedAt);

/// <summary>
/// Per-module analytics summary (from CourseAnalytics read model JSONB).
/// </summary>
public sealed record ModuleAnalyticsSummaryDto(
    Guid ModuleId,
    int LessonCount,
    TimeSpan TotalDuration);

/// <summary>
/// Placeholder for time-series enrollment data (e.g. last 30 days).
/// </summary>
public sealed record EnrollmentCountByDayDto(DateTimeOffset Date, int Count);
