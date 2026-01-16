namespace CoursePlatform.Contracts.AuthEvents;

public sealed record UserRegisteredEvent(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? Email);
