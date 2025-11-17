using Domain.Users.Primitives;
using SharedKernel;

namespace Domain.Users.Events;

public sealed record UserProfileCreatedDomainEvent(
    UserId UserId,
    AuthUserId AuthUserId,
    string Email,
    DateTime CreatedAt
) : IDomainEvent;
