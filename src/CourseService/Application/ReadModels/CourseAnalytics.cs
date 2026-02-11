namespace Courses.Application.ReadModels;

public sealed class CourseAnalytics
{
    public Guid CourseId { get; set; }
    public int TotalLessonsCount { get; set; }
    public TimeSpan TotalCourseDuration { get; set; }
    public double AverageRating { get; set; }
    public int ReviewsCount { get; set; }
    public int EnrollmentsCount { get; set; }

    public List<ModuleAnalytics> ModuleAnalytics { get; set; } = [];
}
