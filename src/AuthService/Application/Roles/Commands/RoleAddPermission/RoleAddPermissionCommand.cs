using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.RoleAddPermission;

public record AddRolePermissionCommand(
    Guid RoleId,
    string Effect,
    string Action, 
    string Resource, 
    string? ResourceId = null) : ICommand;
