using Domain.AuthUsers.Primitives;
using SharedKernel;

namespace Domain.AuthUsers.Events;

public sealed record UserRegisteredDomainEvent(
    AuthUserId AuthUserId,
    int UserId,
    string Email,
    DateTime RegisteredAt
) : IDomainEvent;
