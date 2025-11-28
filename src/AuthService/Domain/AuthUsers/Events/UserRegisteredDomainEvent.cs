using SharedKernel;

namespace Domain.AuthUsers.Events;

public sealed record UserRegisteredDomainEvent(
    Guid AuthUserId,
    string Email,
    DateTime RegisteredAt
) : IDomainEvent;
