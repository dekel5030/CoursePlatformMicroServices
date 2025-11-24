namespace Users.Contracts.Events;

public sealed record UserUpsertedV1(
    int UserId,
    string Fullname,
    string Email,
    bool IsActive,
    long EntityVersion)
{
    public const int Version = 1;
}
