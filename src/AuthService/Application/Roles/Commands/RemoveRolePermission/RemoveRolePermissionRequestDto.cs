namespace Auth.Application.Roles.Commands.RemoveRolePermission;

public record RemoveRolePermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string ResourceId
);