namespace Auth.Contracts.Events;

public sealed record UserRegistered(
    string AuthUserId,
    string UserId, // Same as AuthUserId - unified ID for user across services
    string Email,
    DateTime RegisteredAt
);
