namespace CoursePlatform.Contracts.CourseEvents;

public record CourseModuleAddedIntegrationEvent(
    Guid CourseId,
    Guid ModuleId,
    string Title,
    int Index);
