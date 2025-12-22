using Domain.Users.Primitives;
using Kernel.Messaging.Abstractions;

namespace Domain.Users.Events;

public sealed record UserProfileCreatedDomainEvent(
    UserId UserId,
    AuthUserId AuthUserId,
    string Email,
    DateTime CreatedAt
) : IDomainEvent;