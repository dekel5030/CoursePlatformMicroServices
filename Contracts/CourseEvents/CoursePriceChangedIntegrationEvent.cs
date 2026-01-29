namespace CoursePlatform.Contracts.CourseEvents;

public record CoursePriceChangedIntegrationEvent(Guid CourseId, decimal NewAmount, string NewCurrency);
