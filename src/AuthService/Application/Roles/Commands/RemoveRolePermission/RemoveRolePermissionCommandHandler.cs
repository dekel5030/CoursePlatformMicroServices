using Auth.Application.Abstractions.Data;
using Auth.Domain.Permissions;
using Auth.Domain.Roles;
using Auth.Domain.Roles.Errors;
using Auth.Domain.Roles.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Roles.Commands.RemoveRolePermission;

public class RemoveRolePermissionCommandHandler : ICommandHandler<RemoveRolePermissionCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRolePermissionCommandHandler(
        IWriteDbContext dbContext, 
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RemoveRolePermissionCommand request, 
        CancellationToken cancellationToken = default)
    {
        var permissionResult = Permission.Parse(
            request.Effect,
            request.Action, 
            request.Resource, 
            request.ResourceId);

        if (permissionResult.IsFailure)
        {
            return Result.Failure(permissionResult.Error);
        }

        Permission permission = permissionResult.Value;

        Role? role = await _dbContext.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == new RoleId(request.RoleId), cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        Result result = role.RemovePermission(permission);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
}
