namespace Users.Contracts.Events;

public sealed record UserUpsertedV1(
    int UserId,
    bool IsActive
);