using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.UserRemovePermission;

public record UserRemovePermissionCommand(
    Guid UserId,
    string Effect,
    string Action,
    string Resource,
    string ResourceId) : ICommand;
