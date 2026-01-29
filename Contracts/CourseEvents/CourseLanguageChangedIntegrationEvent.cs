namespace CoursePlatform.Contracts.CourseEvents;

public record CourseLanguageChangedIntegrationEvent(Guid CourseId, string NewLanguage);
