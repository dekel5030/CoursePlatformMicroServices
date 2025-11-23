using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.RefreshToken;

public record RefreshAccessTokenCommand(string RefreshToken) : ICommand<string>;
