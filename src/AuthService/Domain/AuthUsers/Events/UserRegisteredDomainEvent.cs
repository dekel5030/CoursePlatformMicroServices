using SharedKernel;

namespace Domain.AuthUsers.Events;

public sealed record UserRegisteredDomainEvent(
    AuthUser User
) : IDomainEvent;
