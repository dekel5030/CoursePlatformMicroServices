using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Permissions.Primitives;
using Domain.Roles.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.AssignPermissionToRole;

public class AssignPermissionToRoleCommandHandler : ICommandHandler<AssignPermissionToRoleCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public AssignPermissionToRoleCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> Handle(
        AssignPermissionToRoleCommand request,
        CancellationToken cancellationToken = default)
    {
        var role = await _readDbContext.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == new RoleId(request.Request.RoleId), cancellationToken);

        if (role == null)
        {
            return Result.Failure(
                Error.NotFound("Role.NotFound", $"Role with ID {request.Request.RoleId} not found"));
        }

        var permission = await _readDbContext.Permissions
            .FirstOrDefaultAsync(p => p.Id == new PermissionId(request.Request.PermissionId), cancellationToken);

        if (permission == null)
        {
            return Result.Failure(
                Error.NotFound("Permission.NotFound", $"Permission with ID {request.Request.PermissionId} not found"));
        }

        role.AddPermission(new PermissionId(request.Request.PermissionId));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
