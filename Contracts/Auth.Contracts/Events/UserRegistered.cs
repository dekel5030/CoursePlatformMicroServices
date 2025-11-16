namespace Auth.Contracts.Events;

public sealed record UserRegistered(
    string AuthUserId,
    string Email,
    DateTime RegisteredAt
);
