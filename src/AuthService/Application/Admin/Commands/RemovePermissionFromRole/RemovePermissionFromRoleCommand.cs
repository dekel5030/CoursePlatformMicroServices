using Application.Abstractions.Messaging;

namespace Application.Admin.Commands.RemovePermissionFromRole;

public record RemovePermissionFromRoleCommand(int RoleId, int PermissionId) : ICommand;
