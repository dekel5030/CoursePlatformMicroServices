using Auth.Application.Abstractions.Data;
using Auth.Domain.Roles;
using Auth.Domain.Roles.Errors;
using Auth.Domain.Roles.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Roles.Commands.RoleRemovePermission;

public class RoleRemovePermissionCommandHandler : ICommandHandler<RoleRemovePermissionCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public RoleRemovePermissionCommandHandler(
        IWriteDbContext dbContext, 
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RoleRemovePermissionCommand request, 
        CancellationToken cancellationToken = default)
    {
        Role? role = await _dbContext.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Name == new RoleName(request.RoleName), cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        Result result = role.RemovePermission(request.PermissionKey);

        if (result.IsFailure)
        {
            return result;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
}
