using Kernel.Messaging.Abstractions;

namespace Domain.AuthUsers.Events;

public sealed record UserRegisteredDomainEvent(
    AuthUser User
) : IDomainEvent;
