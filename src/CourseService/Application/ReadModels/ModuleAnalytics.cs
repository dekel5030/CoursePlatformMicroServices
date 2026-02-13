namespace Courses.Application.ReadModels;

public sealed class ModuleAnalytics
{
    public Guid ModuleId { get; set; }
    public int LessonCount { get; set; }
    public TimeSpan TotalModuleDuration { get; set; }
}
