using CourseService.Dtos.Courses;

namespace CourseService.Dtos.Lessons;

public class LessonReadDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsPreview { get; set; }

    public int Order { get; set; }      
    public TimeSpan? Duration { get; set; }

    public int CourseId { get; set; }
}