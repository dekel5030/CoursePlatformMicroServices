using Application.Abstractions.Messaging;

namespace Application.Users.IntegrationEvents.AuthRegistered;

public sealed record AuthRegisteredIntegrationEvent(
    string AuthUserId,
    string Email,
    string? Username) : IIntegrationEvent;
