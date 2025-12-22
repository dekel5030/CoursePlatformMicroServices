using Kernel.Messaging.Abstractions;

namespace Application.AuthUsers.Commands.ExchangeToken;
public record ExchangeTokenCommand : ICommand<TokenResponse>;
