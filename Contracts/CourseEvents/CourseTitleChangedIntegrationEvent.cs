namespace CoursePlatform.Contracts.CourseEvents;

public record CourseTitleChangedIntegrationEvent(Guid CourseId, string NewTitle);
