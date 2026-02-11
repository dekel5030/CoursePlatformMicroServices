namespace Courses.Application.ReadModels;

/// <summary>
/// Read-model value for per-module analytics, stored in CourseAnalytics as JSONB.
/// </summary>
public sealed class ModuleAnalytics
{
    public Guid ModuleId { get; set; }
    public int LessonCount { get; set; }
    public TimeSpan TotalModuleDuration { get; set; }
}
