using Kernel.Messaging.Abstractions;

namespace Users.Domain.Users.Events;

public sealed record UserProfileCreatedDomainEvent(User User) : IDomainEvent;
