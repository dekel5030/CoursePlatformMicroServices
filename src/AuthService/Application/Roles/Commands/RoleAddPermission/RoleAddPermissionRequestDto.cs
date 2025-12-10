namespace Application.Roles.Commands.RoleAddPermission;

public record RoleAddPermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);