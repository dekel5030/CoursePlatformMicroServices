namespace Application.Roles.RemoveRolePermission;

public record RemoveRolePermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string ResourceId
);