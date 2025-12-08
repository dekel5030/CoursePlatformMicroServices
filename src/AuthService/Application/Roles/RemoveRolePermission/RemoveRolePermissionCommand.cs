using Application.Abstractions.Messaging;

namespace Application.Roles.RemoveRolePermission;

public record RemoveRolePermissionCommand(
    Guid RoleId,
    string Effect,
    string Action,
    string Resource,
    string ResourceId) : ICommand;
