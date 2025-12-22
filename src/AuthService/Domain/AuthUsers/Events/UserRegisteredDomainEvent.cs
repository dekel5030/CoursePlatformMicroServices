using Kernel.Messaging.Abstractions;

namespace Auth.Domain.AuthUsers.Events;

public sealed record UserRegisteredDomainEvent(
    AuthUser User
) : IDomainEvent;
