using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.AddRole;

public record UserAddRoleCommand(Guid UserId, string RoleName) : ICommand;
