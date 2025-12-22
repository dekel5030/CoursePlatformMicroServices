using Kernel.Messaging.Abstractions;

namespace Application.AuthUsers.Commands.UserRemoveRole;

public record UserRemoveRoleCommand(Guid UserId, string RoleName) : ICommand;
