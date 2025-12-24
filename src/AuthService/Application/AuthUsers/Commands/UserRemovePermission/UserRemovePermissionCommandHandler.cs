using Auth.Application.Abstractions.Data;
using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Errors;
using Auth.Domain.AuthUsers.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.AuthUsers.Commands.UserRemovePermission;

public class UserRemovePermissionCommandHandler : ICommandHandler<UserRemovePermissionCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public UserRemovePermissionCommandHandler(
        IUnitOfWork unitOfWork, 
        IWriteDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(
        UserRemovePermissionCommand request, 
        CancellationToken cancellationToken = default)
    {
        AuthUser? user = await _dbContext.Users.Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Id == new AuthUserId(request.UserId), cancellationToken);

        if (user is null)
        {
            return Result.Failure(AuthUserErrors.NotFound);
        }

        Result removePermissionResult = user.RemovePermission(request.PermissionKey);

        if (removePermissionResult.IsFailure)
        {
            return removePermissionResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
