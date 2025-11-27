using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Commands.CreatePermission;

public record CreatePermissionCommand(CreatePermissionRequest Request) : ICommand<PermissionDto>;
