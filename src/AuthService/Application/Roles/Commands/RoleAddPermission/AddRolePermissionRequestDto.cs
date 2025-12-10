namespace Application.Roles.Commands.AddRolePermission;

public record AddRolePermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);