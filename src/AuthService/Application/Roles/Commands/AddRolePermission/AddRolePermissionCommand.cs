using Kernel.Messaging.Abstractions;

namespace Auth.Application.Roles.Commands.AddRolePermission;

public record AddRolePermissionCommand(
    Guid RoleId,
    string Effect,
    string Action, 
    string Resource, 
    string? ResourceId = null) : ICommand;
