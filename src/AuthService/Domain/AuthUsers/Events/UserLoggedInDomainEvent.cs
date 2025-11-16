using Domain.AuthUsers.Primitives;
using SharedKernel;

namespace Domain.AuthUsers.Events;

public sealed record UserLoggedInDomainEvent(
    AuthUserId AuthUserId,
    string Email,
    DateTime LoggedInAt
) : IDomainEvent;
