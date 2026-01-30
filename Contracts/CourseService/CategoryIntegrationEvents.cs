namespace CoursePlatform.Contracts.CourseService;

public sealed record CategoryCreatedIntegrationEvent(Guid Id, string Name, string Slug);
public sealed record CategoryRenamedIntegrationEvent(Guid Id, string NewName, string NewSlug);