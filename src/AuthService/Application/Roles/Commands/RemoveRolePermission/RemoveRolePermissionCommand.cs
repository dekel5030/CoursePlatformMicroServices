using Kernel.Messaging.Abstractions;

namespace Auth.Application.Roles.Commands.RemoveRolePermission;

public record RemoveRolePermissionCommand(
    Guid RoleId,
    string Effect,
    string Action,
    string Resource,
    string ResourceId) : ICommand;
