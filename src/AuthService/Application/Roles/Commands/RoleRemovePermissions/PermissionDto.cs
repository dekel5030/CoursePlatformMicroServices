namespace Application.Roles.Commands.RoleRemovePermissions;

public record PermissionDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);
