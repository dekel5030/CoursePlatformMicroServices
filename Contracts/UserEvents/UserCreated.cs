namespace CoursePlatform.Contracts.UserEvents;

public sealed record UserCreated(
    string UserId,
    string Fullname,
    string Email,
    DateTime CreatedAt
);