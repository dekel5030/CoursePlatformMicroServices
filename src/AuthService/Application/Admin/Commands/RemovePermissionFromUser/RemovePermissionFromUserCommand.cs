using Application.Abstractions.Messaging;

namespace Application.Admin.Commands.RemovePermissionFromUser;

public record RemovePermissionFromUserCommand(Guid UserId, int PermissionId) : ICommand;
