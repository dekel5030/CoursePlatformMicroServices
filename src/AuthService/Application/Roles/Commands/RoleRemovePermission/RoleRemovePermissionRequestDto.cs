namespace Application.Roles.Commands.RoleRemovePermission;

public record RoleRemovePermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string ResourceId
);