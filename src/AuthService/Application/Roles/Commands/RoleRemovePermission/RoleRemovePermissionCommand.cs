using Kernel.Messaging.Abstractions;

namespace Auth.Application.Roles.Commands.RoleRemovePermission;

public record RoleRemovePermissionCommand(
    string RoleName,
    string PermissionKey) : ICommand;
