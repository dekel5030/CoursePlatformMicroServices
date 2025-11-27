using Application.Abstractions.Messaging;

namespace Application.Admin.Commands.RemoveRoleFromUser;

public record RemoveRoleFromUserCommand(Guid UserId, int RoleId) : ICommand;
