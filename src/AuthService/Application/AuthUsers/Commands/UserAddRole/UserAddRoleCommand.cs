using Kernel.Messaging.Abstractions;

namespace Application.AuthUsers.Commands.UserAddRole;

public record UserAddRoleCommand(Guid UserId, string RoleName) : ICommand;
