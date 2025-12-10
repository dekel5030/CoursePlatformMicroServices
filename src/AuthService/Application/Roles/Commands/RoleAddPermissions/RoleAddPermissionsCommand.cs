using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.RoleAddPermissions;

public record RoleAddPermissionsCommand(
    Guid RoleId,
    List<PermissionDto> Permissions) : ICommand;
