using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.UserAddPermissions;

public record UserAddPermissionsCommand(
    Guid UserId,
    List<PermissionDto> Permissions) : ICommand;
