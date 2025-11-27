using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Commands.CreateRole;

public record CreateRoleCommand(CreateRoleRequest Request) : ICommand<RoleDto>;
