namespace CourseService.Models;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public int? InstructorUserId { get; set; }

    public bool IsPublished { get; set; } = false;

    public decimal Price { get; set; } = 0.0m;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
