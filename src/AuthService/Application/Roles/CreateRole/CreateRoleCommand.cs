using Application.Abstractions.Messaging;

namespace Application.Roles.CreateRole;

public record CreateRoleCommand(string RoleName) : ICommand<CreateRoleResponseDto>;
