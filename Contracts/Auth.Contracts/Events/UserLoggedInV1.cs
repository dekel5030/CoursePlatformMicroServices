namespace Auth.Contracts.Events;

public sealed record UserLoggedInV1(
    int AuthUserId,
    string Email,
    DateTime LoggedInAt
);
