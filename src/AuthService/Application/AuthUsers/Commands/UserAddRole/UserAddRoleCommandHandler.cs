using Auth.Application.Abstractions.Data;
using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Errors;
using Auth.Domain.AuthUsers.Primitives;
using Auth.Domain.Roles;
using Auth.Domain.Roles.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.AuthUsers.Commands.UserAddRole;

public class UserAddRoleCommandHandler : ICommandHandler<UserAddRoleCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public UserAddRoleCommandHandler(
        IWriteDbContext dbContext, 
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UserAddRoleCommand request, 
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

        Result addRoleResult = user.AddRole(role);

        if (addRoleResult.IsFailure)
        {
            return addRoleResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
