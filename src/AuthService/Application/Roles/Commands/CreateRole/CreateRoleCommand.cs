using Application.Abstractions.Messaging;

namespace Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(string RoleName) : ICommand<CreateRoleResponseDto>;
