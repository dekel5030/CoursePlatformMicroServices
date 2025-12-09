using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.UserRemoveRole;

public record UserRemoveRoleCommand(Guid UserId, string RoleName) : ICommand;
