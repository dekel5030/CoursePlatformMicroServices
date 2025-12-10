namespace Application.Roles.Commands.RoleAddPermission;

public record AddRolePermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);