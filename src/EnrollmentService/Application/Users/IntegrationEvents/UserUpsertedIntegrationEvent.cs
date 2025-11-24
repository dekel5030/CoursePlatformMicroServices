using Application.Abstractions.Messaging;

namespace Application.Users.IntegrationEvents;

public sealed record UserUpsertedIntegrationEvent(
    int UserId,
    string Name,
    bool IsActive) : IIntegrationEvent;
