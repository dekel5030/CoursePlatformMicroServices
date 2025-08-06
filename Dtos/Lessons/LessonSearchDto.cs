namespace CourseService.Dtos.Lessons;

public class LessonSearchDto
{
    public int CourseId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}