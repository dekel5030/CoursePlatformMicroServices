namespace Application.Roles.AddRolePermission;

public record AddRolePermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);