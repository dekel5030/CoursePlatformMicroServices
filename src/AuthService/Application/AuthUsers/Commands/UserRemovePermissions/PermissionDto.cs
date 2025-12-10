namespace Application.AuthUsers.Commands.UserRemovePermissions;

public record PermissionDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);
