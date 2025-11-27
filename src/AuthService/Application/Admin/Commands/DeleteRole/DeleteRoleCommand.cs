using Application.Abstractions.Messaging;

namespace Application.Admin.Commands.DeleteRole;

public record DeleteRoleCommand(int RoleId) : ICommand;
