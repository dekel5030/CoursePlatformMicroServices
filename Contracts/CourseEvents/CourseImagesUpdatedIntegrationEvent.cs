namespace CoursePlatform.Contracts.CourseEvents;

public record CourseImagesUpdatedIntegrationEvent(
    Guid CourseId,
    List<string> ImageUrls);
