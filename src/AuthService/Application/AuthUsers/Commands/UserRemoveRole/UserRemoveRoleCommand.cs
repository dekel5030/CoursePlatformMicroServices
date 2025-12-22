using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Commands.UserRemoveRole;

public record UserRemoveRoleCommand(Guid UserId, string RoleName) : ICommand;
