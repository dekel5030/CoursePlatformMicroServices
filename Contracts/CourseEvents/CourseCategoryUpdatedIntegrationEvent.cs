namespace CoursePlatform.Contracts.CourseEvents;

public record CourseCategoryUpdatedIntegrationEvent(
    Guid CourseId,
    Guid CategoryId,
    string CategoryName,
    string CategorySlug);
