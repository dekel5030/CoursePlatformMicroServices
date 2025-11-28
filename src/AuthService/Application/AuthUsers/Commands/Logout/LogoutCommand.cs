using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.Logout;

public record LogoutCommand(string Email, string RefreshToken) : ICommand;
