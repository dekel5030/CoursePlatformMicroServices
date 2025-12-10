using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.RoleRemovePermission;

public record RemoveRolePermissionCommand(
    Guid RoleId,
    string Effect,
    string Action,
    string Resource,
    string ResourceId) : ICommand;
