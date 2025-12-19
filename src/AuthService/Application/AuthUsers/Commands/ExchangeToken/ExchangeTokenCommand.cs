using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.ExchangeToken;
public record ExchangeTokenCommand : ICommand<TokenResponse>;
