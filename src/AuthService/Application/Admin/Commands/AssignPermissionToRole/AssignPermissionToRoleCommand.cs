using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Commands.AssignPermissionToRole;

public record AssignPermissionToRoleCommand(AssignPermissionToRoleRequest Request) : ICommand;
