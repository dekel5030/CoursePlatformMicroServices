using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Permissions;
using Domain.Roles.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Commands.RoleRemovePermissions;

public class RoleRemovePermissionsCommandHandler : ICommandHandler<RoleRemovePermissionsCommand>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly IUnitOfWork _unitOfWork;

    public RoleRemovePermissionsCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteDbContext writeDbContext)
    {
        _unitOfWork = unitOfWork;
        _writeDbContext = writeDbContext;
    }

    public async Task<Result> Handle(
        RoleRemovePermissionsCommand request, 
        CancellationToken cancellationToken = default)
    {
        var role = await _writeDbContext.Roles
                    .Include(r => r.Permissions)
                    .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        var permissions = new List<Permission>();
        var errors = new List<Error>();

        foreach (var permissionDto in request.Permissions)
        {
            Result<Permission> permissionParseResult = Permission.Parse(
                permissionDto.Effect, 
                permissionDto.Action, 
                permissionDto.Resource, 
                permissionDto.ResourceId);

            if (permissionParseResult.IsFailure)
            {
                errors.Add(permissionParseResult.Error);
            }
            else
            {
                permissions.Add(permissionParseResult.Value);
            }
        }

        if (errors.Count > 0)
        {
            return Result.Failure(new ValidationError(errors));
        }

        var permissionRemoveResult = role.RemovePermissions(permissions);

        if (permissionRemoveResult.IsFailure)
        {
            return permissionRemoveResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
