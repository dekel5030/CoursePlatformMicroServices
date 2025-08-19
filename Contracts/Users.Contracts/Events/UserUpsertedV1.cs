namespace Users.Contracts.Events;

public sealed record UserUpsertedV1(
    int UserId,
    bool IsActive
)
{
    public const int Version = 1;
};