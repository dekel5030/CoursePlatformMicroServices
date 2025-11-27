using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Permissions.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.DeletePermission;

public class DeletePermissionCommandHandler : ICommandHandler<DeletePermissionCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public DeletePermissionCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> Handle(
        DeletePermissionCommand request,
        CancellationToken cancellationToken = default)
    {
        var permission = await _readDbContext.Permissions
            .FirstOrDefaultAsync(p => p.Id == new PermissionId(request.PermissionId), cancellationToken);

        if (permission == null)
        {
            return Result.Failure(
                Error.NotFound("Permission.NotFound", $"Permission with ID {request.PermissionId} not found"));
        }

        _dbContext.Permissions.Remove(permission);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
