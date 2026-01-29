namespace CoursePlatform.Contracts.CourseEvents;

public record CourseCreatedIntegrationEvent(
    Guid CourseId,
    Guid InstructorId,
    string Title,
    string Description,
    decimal PriceAmount,
    string PriceCurrency,
    string Status,
    string Slug,
    string Difficulty,
    string Language,
    Guid CategoryId);
