namespace CourseService.Dtos.Courses;

public class CourseSearchDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }

    public int? InstructorUserId { get; set; }

    public bool? IsPublished { get; set; } 

    public decimal? Price { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
