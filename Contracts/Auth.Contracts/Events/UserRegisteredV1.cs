namespace Auth.Contracts.Events;

public sealed record UserRegisteredV1(
    int AuthUserId,
    int UserId,
    string Email,
    DateTime RegisteredAt
);
