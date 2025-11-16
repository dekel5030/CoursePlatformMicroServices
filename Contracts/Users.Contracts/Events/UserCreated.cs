namespace Users.Contracts.Events;

public sealed record UserCreated(
    string UserId,
    string Fullname,
    string Email,
    DateTime CreatedAt
);