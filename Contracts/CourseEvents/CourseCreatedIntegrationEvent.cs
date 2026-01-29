namespace CoursePlatform.Contracts.CourseEvents;

public record CourseCreatedIntegrationEvent
{
    public required Guid CourseId { get; init; }
    public required Guid InstructorId { get; init; }
    public required string InstructorName { get; init; }
    public required string Title { get; init; }
    public required string Slug { get; init; }
    public required decimal PriceAmount { get; init; }
    public required string PriceCurrency { get; init; }
    public required string Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
