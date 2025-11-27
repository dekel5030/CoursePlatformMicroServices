using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.AuthUsers.Primitives;
using Domain.Permissions.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.AssignPermissionToUser;

public class AssignPermissionToUserCommandHandler : ICommandHandler<AssignPermissionToUserCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public AssignPermissionToUserCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> Handle(
        AssignPermissionToUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _readDbContext.AuthUsers
            .Include(u => u.UserPermissions)
            .FirstOrDefaultAsync(u => u.Id == new AuthUserId(request.Request.UserId), cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {request.Request.UserId} not found"));
        }

        var permission = await _readDbContext.Permissions
            .FirstOrDefaultAsync(p => p.Id == new PermissionId(request.Request.PermissionId), cancellationToken);

        if (permission == null)
        {
            return Result.Failure(
                Error.NotFound("Permission.NotFound", $"Permission with ID {request.Request.PermissionId} not found"));
        }

        user.AddPermission(new PermissionId(request.Request.PermissionId));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
