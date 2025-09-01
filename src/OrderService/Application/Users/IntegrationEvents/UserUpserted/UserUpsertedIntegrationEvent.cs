using Application.Abstractions.Messaging;

namespace Application.Users.IntegrationEvents.UserUpserted;

public sealed record UserUpsertedIntegrationEvent(
    string UserId,
    string Email,
    string Fullname,
    bool IsActive,
    long AggregateVersion) : IIntegrationEvent;