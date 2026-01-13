using Kernel.Messaging.Abstractions;
using Users.Domain.Users.Primitives;

namespace Users.Domain.Users.Events;

public sealed record UserProfileCreatedDomainEvent(
    UserId UserId,
    AuthUserId AuthUserId,
    string Email,
    DateTime CreatedAt
) : IDomainEvent;