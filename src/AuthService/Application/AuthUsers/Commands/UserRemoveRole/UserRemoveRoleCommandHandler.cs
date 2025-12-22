using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.AuthUsers.Primitives;
using Domain.Roles;
using Domain.Roles.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.UserRemoveRole;

public class UserRemoveRoleCommandHandler : ICommandHandler<UserRemoveRoleCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public UserRemoveRoleCommandHandler(
        IWriteDbContext dbContext, 
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UserRemoveRoleCommand request, 
        CancellationToken cancellationToken = default)
    {
        AuthUser? user = await _dbContext.Users.Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == new AuthUserId(request.UserId));

        if (user is null)
        {
            return Result.Failure(AuthUserErrors.NotFound);
        }

        Role? role = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == request.RoleName, cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        Result removeRoleResult = user.RemoveRole(role);

        if (removeRoleResult.IsFailure)
        {
            return removeRoleResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
