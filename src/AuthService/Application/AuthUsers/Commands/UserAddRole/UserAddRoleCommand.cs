using Application.Abstractions.Messaging;

namespace Application.AuthUsers.Commands.UserAddRole;

public record UserAddRoleCommand(Guid UserId, string RoleName) : ICommand;
