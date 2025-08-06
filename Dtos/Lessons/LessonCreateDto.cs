namespace CourseService.Dtos.Lessons;

public class LessonCreateDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }

    public int CourseId { get; set; }
}