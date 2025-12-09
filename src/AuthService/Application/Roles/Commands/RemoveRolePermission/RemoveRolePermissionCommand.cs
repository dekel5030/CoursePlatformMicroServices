using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.RemoveRolePermission;

public record RemoveRolePermissionCommand(
    Guid RoleId,
    string Effect,
    string Action,
    string Resource,
    string ResourceId) : ICommand;
