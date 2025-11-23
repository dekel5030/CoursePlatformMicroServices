using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.Logout;

public record LogoutCommand(string RefreshToken) : ICommand;
