namespace Auth.Contracts.Events;

public sealed record UserRegistered(
    string AuthUserId,
    string UserId,
    string Email,
    DateTime RegisteredAt
);
