using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.ExchangeToken;
internal record ExchangeTokenCommand : ICommand<TokenResponse>;
