using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Abstractions.Data.ReadModels;

public sealed class ModuleDetailsReadModel
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Index { get; set; }
    public int LessonCount { get; set; }
    public double TotalDurationSeconds { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
    public List<LessonSummaryReadModel> Lessons { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only
}

public sealed class LessonSummaryReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Index { get; set; }
    public TimeSpan Duration { get; set; }
    public string? ThumbnailUrl { get; set; }
    public LessonAccess Access { get; set; }
}
