using Application.Abstractions.Messaging;

namespace Application.Admin.Commands.DeletePermission;

public record DeletePermissionCommand(int PermissionId) : ICommand;
