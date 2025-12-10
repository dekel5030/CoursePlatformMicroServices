using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.UserRemovePermissions;

public record UserRemovePermissionsCommand(
    Guid UserId,
    List<PermissionDto> Permissions) : ICommand;

public record PermissionDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);
