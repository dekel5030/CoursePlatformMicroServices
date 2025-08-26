using System.Text.Json.Serialization;
using CourseService.Dtos.Lessons;

namespace CourseService.Dtos.Courses;

public class CourseReadDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public int? InstructorUserId { get; set; }

    public bool IsPublished { get; set; } 

    public decimal Price { get; set; } 
    public ICollection<LessonReadDto>? Lessons { get; set; }
}
