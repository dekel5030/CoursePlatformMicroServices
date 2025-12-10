using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.RemovePermissionsFromRole;

public record RemovePermissionsFromRoleCommand(
    Guid RoleId,
    List<PermissionDto> Permissions) : ICommand;

public record PermissionDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);
