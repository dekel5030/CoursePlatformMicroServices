using Domain.AuthUsers.Primitives;
using SharedKernel;

namespace Domain.AuthUsers.Events;

public sealed record UserRegisteredDomainEvent(
    AuthUserId AuthUserId,
    string Email,
    DateTime RegisteredAt
) : IDomainEvent;
