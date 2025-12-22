namespace Auth.Application.AuthUsers.Commands.UserAddPermission;

public record UserAddPermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string ResourceId);
