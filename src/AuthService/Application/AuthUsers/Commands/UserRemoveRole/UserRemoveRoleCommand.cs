using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.RemoveRole;

public record UserRemoveRoleCommand(Guid UserId, string RoleName) : ICommand;
