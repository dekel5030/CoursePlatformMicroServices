using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Roles.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.DeleteRole;

public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public DeleteRoleCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> Handle(
        DeleteRoleCommand request,
        CancellationToken cancellationToken = default)
    {
        var role = await _readDbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == new RoleId(request.RoleId), cancellationToken);

        if (role == null)
        {
            return Result.Failure(
                Error.NotFound("Role.NotFound", $"Role with ID {request.RoleId} not found"));
        }

        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
