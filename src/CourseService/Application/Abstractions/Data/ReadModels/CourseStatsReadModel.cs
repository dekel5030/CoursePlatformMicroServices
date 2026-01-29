namespace Courses.Application.Abstractions.Data.ReadModels;

public sealed class CourseStatsReadModel
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public int LessonsCount { get; set; }
    public int ModulesCount { get; set; }
    public int EnrollmentCount { get; set; }
}
