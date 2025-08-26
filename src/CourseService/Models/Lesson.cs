namespace CourseService.Models;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsPreview { get; set; } = false;

    public int Order { get; set; } = 1;
    public TimeSpan? Duration { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; } 
}