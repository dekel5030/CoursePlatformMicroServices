using Auth.Application.Abstractions.Data;
using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Errors;
using Auth.Domain.AuthUsers.Primitives;
using Auth.Domain.Permissions;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.AuthUsers.Commands.UserAddPermission;

public class UserAddPermissionCommandHandler : ICommandHandler<UserAddPermissionCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public UserAddPermissionCommandHandler(
        IUnitOfWork unitOfWork, 
        IWriteDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(
        UserAddPermissionCommand request, 
        CancellationToken cancellationToken = default)
    {
        AuthUser? user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == new AuthUserId(request.UserId), cancellationToken);

        if (user is null)
        {
            return Result.Failure(AuthUserErrors.NotFound);
        }

        Result<Permission> permissionResult = Permission.Parse(
            request.Effect,
            request.Action,
            request.Resource,
            request.ResourceId);

        if (permissionResult.IsFailure)
        {
            return Result.Failure(permissionResult.Error);
        }

        Permission permission = permissionResult.Value;
        Result addPermissionResult = user.AddPermission(permission);

        if (addPermissionResult.IsFailure)
        {
            return addPermissionResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
