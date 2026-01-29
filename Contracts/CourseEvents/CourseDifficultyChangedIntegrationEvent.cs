namespace CoursePlatform.Contracts.CourseEvents;

public record CourseDifficultyChangedIntegrationEvent(Guid CourseId, string NewDifficulty);
