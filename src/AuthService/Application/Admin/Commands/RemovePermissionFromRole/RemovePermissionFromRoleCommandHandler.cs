using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Permissions.Primitives;
using Domain.Roles.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.RemovePermissionFromRole;

public class RemovePermissionFromRoleCommandHandler : ICommandHandler<RemovePermissionFromRoleCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public RemovePermissionFromRoleCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> Handle(
        RemovePermissionFromRoleCommand request,
        CancellationToken cancellationToken = default)
    {
        var role = await _readDbContext.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == new RoleId(request.RoleId), cancellationToken);

        if (role == null)
        {
            return Result.Failure(
                Error.NotFound("Role.NotFound", $"Role with ID {request.RoleId} not found"));
        }

        role.RemovePermission(new PermissionId(request.PermissionId));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
