using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.Permissions;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.UserAddPermission;

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
        AuthUser? user = await _dbContext.AuthUsers.Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(AuthUserErrors.NotFound);
        }

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
        Result addPermissionResultt = user.AddPermission(permission);

        if (addPermissionResultt.IsFailure)
        {
            return addPermissionResultt;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
