using Auth.Application.Abstractions.Data;
using Auth.Domain.Permissions;
using Auth.Domain.Roles;
using Auth.Domain.Roles.Errors;
using Auth.Domain.Roles.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Roles.Commands.RoleAddPermission;

public class RoleAddPermissionCommandHandler : ICommandHandler<RoleAddPermissionCommand>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly IUnitOfWork _unitOfWork;

    public RoleAddPermissionCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteDbContext writeDbContext)
    {
        _unitOfWork = unitOfWork;
        _writeDbContext = writeDbContext;
    }

    public async Task<Result> Handle(
        RoleAddPermissionCommand request,
        CancellationToken cancellationToken = default)
    {
        Role? role = await _writeDbContext.Roles
            .FirstOrDefaultAsync(role => role.Name == new RoleName(request.RoleName), cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        Result<Permission> permissionParseResult = Permission.Parse(
            request.Effect,
            request.Action,
            request.Resource,
            request.ResourceId);

        if (permissionParseResult.IsFailure)
        {
            return permissionParseResult;
        }

        Result permissionAddResult = role.AddPermission(permissionParseResult.Value);

        if (permissionAddResult.IsFailure)
        {
            return permissionAddResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
