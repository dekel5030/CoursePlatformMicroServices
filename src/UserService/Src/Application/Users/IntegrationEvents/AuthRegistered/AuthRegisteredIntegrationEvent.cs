using Application.Abstractions.Messaging;

namespace Application.Users.IntegrationEvents.AuthRegistered;

public sealed record AuthRegisteredIntegrationEvent(
    string Email, string? Username) : IIntegrationEvent;