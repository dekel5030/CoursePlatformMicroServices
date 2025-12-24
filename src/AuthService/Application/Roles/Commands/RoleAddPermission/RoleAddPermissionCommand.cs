using Kernel.Messaging.Abstractions;

namespace Auth.Application.Roles.Commands.RoleAddPermission;

public record RoleAddPermissionCommand(
    string RoleName,
    string Effect,
    string Action, 
    string Resource, 
    string ResourceId) : ICommand;
