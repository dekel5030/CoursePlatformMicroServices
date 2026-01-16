using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Commands.ExchangeToken;

public record ExchangeTokenCommand : ICommand<TokenResponse>;
