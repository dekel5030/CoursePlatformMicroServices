namespace CoursePlatform.Contracts.CourseEvents;

public record CourseDescriptionChangedIntegrationEvent(Guid CourseId, string NewDescription);
