using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.AuthUsers.Primitives;
using Domain.Roles.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.AssignRoleToUser;

public class AssignRoleToUserCommandHandler : ICommandHandler<AssignRoleToUserCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public AssignRoleToUserCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> Handle(
        AssignRoleToUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _readDbContext.AuthUsers
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == new AuthUserId(request.Request.UserId), cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {request.Request.UserId} not found"));
        }

        var role = await _readDbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == new RoleId(request.Request.RoleId), cancellationToken);

        if (role == null)
        {
            return Result.Failure(
                Error.NotFound("Role.NotFound", $"Role with ID {request.Request.RoleId} not found"));
        }

        user.AddRole(new RoleId(request.Request.RoleId));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
