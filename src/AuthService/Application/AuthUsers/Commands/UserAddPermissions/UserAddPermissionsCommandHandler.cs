using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.Permissions;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.UserAddPermissions;

public class UserAddPermissionsCommandHandler : ICommandHandler<UserAddPermissionsCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public UserAddPermissionsCommandHandler(
        IUnitOfWork unitOfWork, 
        IWriteDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(
        UserAddPermissionsCommand request, 
        CancellationToken cancellationToken = default)
    {
        AuthUser? user = await _dbContext.AuthUsers
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(AuthUserErrors.NotFound);
        }

        var permissions = new List<Permission>();

        foreach (var permissionDto in request.Permissions)
        {
            var permissionResult = Permission.Parse(
                permissionDto.Effect,
                permissionDto.Action,
                permissionDto.Resource,
                permissionDto.ResourceId);

            if (permissionResult.IsFailure)
            {
                return Result.Failure(permissionResult.Error);
            }

            permissions.Add(permissionResult.Value);
        }

        Result addPermissionsResult = user.AddPermissions(permissions);

        if (addPermissionsResult.IsFailure)
        {
            return addPermissionsResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
