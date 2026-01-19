namespace CoursePlatform.Contracts.UserEvents;

public sealed record UserCreated(
    string UserId,
    string FirstName,
    string LastName,
    string Email,
    string? AvatarUrl
);
