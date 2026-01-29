namespace CoursePlatform.Contracts.CourseEvents;

public record CourseModuleUpdatedIntegrationEvent(
    Guid CourseId,
    Guid ModuleId,
    string Title,
    int Index);
