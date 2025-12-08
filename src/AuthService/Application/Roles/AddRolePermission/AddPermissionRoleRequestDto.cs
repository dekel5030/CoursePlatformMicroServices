namespace Application.Roles.AddRolePermission;

public record AddPermissionRoleRequestDto(
    string Action,
    string Resource,
    string? ResourceId = null);