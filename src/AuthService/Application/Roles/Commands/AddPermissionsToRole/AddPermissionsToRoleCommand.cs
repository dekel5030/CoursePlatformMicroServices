using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.AddPermissionsToRole;

public record AddPermissionsToRoleCommand(
    Guid RoleId,
    List<PermissionDto> Permissions) : ICommand;

public record PermissionDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);
