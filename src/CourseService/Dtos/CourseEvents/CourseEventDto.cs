namespace CourseService.Dtos.CourseEvents;

public class CourseUpsertedEventDto
{
    public int CourseId { get; set; }
    public bool IsPublished { get; set; } = false;
}