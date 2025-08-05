namespace CourseService.Models;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public int? InstructorUserId { get; set; }

    public bool IsPublished { get; set; } = false;

    public double Price { get; set; } = 0.0;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
