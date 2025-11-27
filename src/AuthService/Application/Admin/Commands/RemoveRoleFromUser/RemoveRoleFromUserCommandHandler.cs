using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.AuthUsers.Primitives;
using Domain.Roles.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.RemoveRoleFromUser;

public class RemoveRoleFromUserCommandHandler : ICommandHandler<RemoveRoleFromUserCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public RemoveRoleFromUserCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> Handle(
        RemoveRoleFromUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _readDbContext.AuthUsers
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == new AuthUserId(request.UserId), cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {request.UserId} not found"));
        }

        user.RemoveRole(new RoleId(request.RoleId));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
