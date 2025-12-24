using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Commands.UserRemovePermission;

public record UserRemovePermissionCommand(
    Guid UserId,
    string PermissionKey) : ICommand;
