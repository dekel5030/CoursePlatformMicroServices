namespace Users.Contracts.Events;

public sealed record UserUpsertedV1(
    Guid MessageId,
    Guid CorrelationId,
    int UserId,
    long Version,
    DateTime UpdatedAtUtc
);