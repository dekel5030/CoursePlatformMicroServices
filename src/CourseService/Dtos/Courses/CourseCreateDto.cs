namespace CourseService.Dtos.Courses;

public class CourseCreateDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int? InstructorUserId { get; set; }
    public decimal Price { get; set; }
}