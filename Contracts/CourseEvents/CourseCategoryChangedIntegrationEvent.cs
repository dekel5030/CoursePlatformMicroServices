namespace CoursePlatform.Contracts.CourseEvents;

public record CourseCategoryChangedIntegrationEvent(Guid CourseId, Guid NewCategoryId);
