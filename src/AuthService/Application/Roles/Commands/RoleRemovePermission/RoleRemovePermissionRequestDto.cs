namespace Application.Roles.Commands.RoleRemovePermission;

public record RemoveRolePermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string ResourceId
);