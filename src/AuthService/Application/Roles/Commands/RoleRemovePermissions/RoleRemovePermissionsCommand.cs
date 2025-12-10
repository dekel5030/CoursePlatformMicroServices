using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.RoleRemovePermissions;

public record RoleRemovePermissionsCommand(
    Guid RoleId,
    List<PermissionDto> Permissions) : ICommand;
