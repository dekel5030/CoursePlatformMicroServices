namespace Auth.Application.AuthUsers.Commands.UserRemovePermission;

public record UserRemovePermissionRequestDto(
    string Effect,
    string Action,
    string Resource,
    string ResourceId);
