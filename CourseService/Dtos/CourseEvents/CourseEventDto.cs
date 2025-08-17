namespace CourseService.Dtos.CourseEvents;

public class CourseUpsertedEventDto
{
    public int Id { get; set; }
    public bool IsPublished { get; set; } = false;
}