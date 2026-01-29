namespace CoursePlatform.Contracts.CourseEvents;

public record CourseTagsUpdatedIntegrationEvent(
    Guid CourseId,
    List<string> Tags);
