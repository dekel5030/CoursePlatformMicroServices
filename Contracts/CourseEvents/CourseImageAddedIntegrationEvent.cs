namespace CoursePlatform.Contracts.CourseEvents;

public record CourseImageAddedIntegrationEvent(Guid CourseId, string ImageUrl);
