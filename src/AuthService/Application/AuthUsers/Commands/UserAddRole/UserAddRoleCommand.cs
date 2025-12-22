using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Commands.UserAddRole;

public record UserAddRoleCommand(Guid UserId, string RoleName) : ICommand;
