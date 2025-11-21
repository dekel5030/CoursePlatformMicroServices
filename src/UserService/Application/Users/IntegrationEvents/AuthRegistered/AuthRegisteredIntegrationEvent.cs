using Application.Abstractions.Messaging;

namespace Application.Users.IntegrationEvents.AuthRegistered;

public sealed record AuthRegisteredIntegrationEvent(
    string AuthUserId,
    string UserId, // Same as AuthUserId - unified ID
    string Email,
    string? Username) : IIntegrationEvent;
