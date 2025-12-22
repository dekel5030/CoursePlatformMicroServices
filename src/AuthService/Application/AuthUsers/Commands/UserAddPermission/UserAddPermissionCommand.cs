using Kernel.Messaging.Abstractions;

namespace Application.AuthUsers.Commands.UserAddPermission;

public record UserAddPermissionCommand(
    Guid UserId,
    string Effect,
    string Action,
    string Resource,
    string ResourceId) : ICommand;
