namespace Users.Contracts.Events;

public sealed record UserCreatedV1(
    string AuthUserId,
    string UserId,
    DateTime CreatedAt
);
