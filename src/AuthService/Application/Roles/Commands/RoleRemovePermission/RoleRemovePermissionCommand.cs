using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.RoleRemovePermission;

public record RoleRemovePermissionCommand(
    Guid RoleId,
    string Effect,
    string Action,
    string Resource,
    string ResourceId) : ICommand;
