using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.RefreshAccessToken;

public record RefreshAccessTokenCommand(string RefreshToken) : ICommand<string>;
