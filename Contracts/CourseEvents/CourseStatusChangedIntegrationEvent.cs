namespace CoursePlatform.Contracts.CourseEvents;

public record CourseStatusChangedIntegrationEvent(Guid CourseId, string NewStatus);
