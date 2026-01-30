using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Abstractions.Data.ReadModels;

public sealed class LessonDetailsReadModel
{
    public Guid LessonId { get; set; }
    public Guid ModuleId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Index { get; set; }
    public TimeSpan Duration { get; set; }
    public string? ThumbnailUrl { get; set; }
    public LessonAccess Access { get; set; }
    public string? VideoUrl { get; set; }
    public string? TranscriptUrl { get; set; }
}
