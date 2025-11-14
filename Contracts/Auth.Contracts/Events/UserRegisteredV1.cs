namespace Auth.Contracts.Events;

public sealed record UserRegisteredV1(
    string AuthUserId,
    string Email,
    DateTime RegisteredAt,
    IEnumerable<string> Roles,
    IEnumerable<string> Permissions
);
