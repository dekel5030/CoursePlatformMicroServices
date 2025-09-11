namespace Users.Contracts.Events;

public sealed record UserUpsertedV1(
    string UserId,
    string Email,
    string Fullname,
    bool IsActive,
    long EntityVersion
);