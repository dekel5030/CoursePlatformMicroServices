using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;

namespace Application.AuthUsers.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<AuthTokensDto>;
