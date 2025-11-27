using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Commands.AssignPermissionToUser;

public record AssignPermissionToUserCommand(AssignPermissionToUserRequest Request) : ICommand;
