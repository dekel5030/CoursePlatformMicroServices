using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.Roles;
using Domain.Roles.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.AddRole;

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
        AuthUser? user = await _dbContext.AuthUsers.Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

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
