using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.RoleAddPermission;

public record RoleAddPermissionCommand(
    Guid RoleId,
    string Effect,
    string Action, 
    string Resource, 
    string? ResourceId = null) : ICommand;
