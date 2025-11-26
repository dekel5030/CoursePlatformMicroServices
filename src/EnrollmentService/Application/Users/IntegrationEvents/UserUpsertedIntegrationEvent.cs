using Application.Abstractions.Messaging;

namespace Application.Users.IntegrationEvents;

public sealed record UserUpsertedIntegrationEvent(
    string UserId,
    string Name,
    bool IsActive) : IIntegrationEvent;
