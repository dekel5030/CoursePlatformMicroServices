using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Commands.AssignRoleToUser;

public record AssignRoleToUserCommand(AssignRoleToUserRequest Request) : ICommand;
