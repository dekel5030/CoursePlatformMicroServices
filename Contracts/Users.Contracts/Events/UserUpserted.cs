namespace Users.Contracts.Events;

public sealed record UserUpserted(
    string UserId,
    string Fullname,
    string Email,
    bool IsActive);