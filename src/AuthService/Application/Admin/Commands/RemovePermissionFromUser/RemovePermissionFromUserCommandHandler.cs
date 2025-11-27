using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.AuthUsers.Primitives;
using Domain.Permissions.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.RemovePermissionFromUser;

public class RemovePermissionFromUserCommandHandler : ICommandHandler<RemovePermissionFromUserCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public RemovePermissionFromUserCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> Handle(
        RemovePermissionFromUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _readDbContext.AuthUsers
            .Include(u => u.UserPermissions)
            .FirstOrDefaultAsync(u => u.Id == new AuthUserId(request.UserId), cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {request.UserId} not found"));
        }

        user.RemovePermission(new PermissionId(request.PermissionId));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
