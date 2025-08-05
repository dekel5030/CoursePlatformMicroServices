namespace CourseService.Dtos;

public class CourseSearchDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public int? InstructorUserId { get; set; }

    public bool? IsPublished { get; set; } 

    public double? Price { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
