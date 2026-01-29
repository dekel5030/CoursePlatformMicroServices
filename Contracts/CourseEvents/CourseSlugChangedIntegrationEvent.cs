namespace CoursePlatform.Contracts.CourseEvents;

public record CourseSlugChangedIntegrationEvent(Guid CourseId, string NewSlug);
