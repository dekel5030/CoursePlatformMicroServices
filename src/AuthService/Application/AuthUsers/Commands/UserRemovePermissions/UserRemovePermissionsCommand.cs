using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.UserRemovePermissions;

public record UserRemovePermissionsCommand(
    Guid UserId,
    List<PermissionDto> Permissions) : ICommand;
