namespace Application.AuthUsers.Commands.UserAddPermissions;

public record PermissionDto(
    string Effect,
    string Action,
    string Resource,
    string ResourceId);
