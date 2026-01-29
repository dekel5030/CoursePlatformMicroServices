namespace CoursePlatform.Contracts.CourseEvents;

public record CourseDetailsUpdatedIntegrationEvent
{
    public required Guid CourseId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required decimal PriceAmount { get; init; }
    public required string PriceCurrency { get; init; }
    public required DateTimeOffset UpdatedAtUtc { get; init; }
}
