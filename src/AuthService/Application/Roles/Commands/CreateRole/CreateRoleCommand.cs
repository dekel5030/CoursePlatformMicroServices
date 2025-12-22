using Kernel.Messaging.Abstractions;

namespace Auth.Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(string RoleName) : ICommand<CreateRoleResponseDto>;
