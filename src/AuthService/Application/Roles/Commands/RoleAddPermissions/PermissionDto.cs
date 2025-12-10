namespace Application.Roles.Commands.RoleAddPermissions;

public record PermissionDto(
    string Effect,
    string Action,
    string Resource,
    string ResourceId);
