using Kernel.Messaging.Abstractions;

namespace Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(string RoleName) : ICommand<CreateRoleResponseDto>;
