using Application.Abstractions.Messaging;

namespace Application.Roles.AddRolePermission;

public record AddRolePermissionCommand(
    Guid RoleId, 
    string Action, 
    string Resource, 
    string? ResourceId = null) : ICommand;
