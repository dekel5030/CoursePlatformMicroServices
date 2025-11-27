using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Admin.Dtos;
using Domain.Permissions;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.CreatePermission;

public class CreatePermissionCommandHandler : ICommandHandler<CreatePermissionCommand, PermissionDto>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public CreatePermissionCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result<PermissionDto>> Handle(
        CreatePermissionCommand request,
        CancellationToken cancellationToken = default)
    {
        var existingPermission = await _readDbContext.Permissions
            .FirstOrDefaultAsync(p => p.Name == request.Request.Name, cancellationToken);

        if (existingPermission != null)
        {
            return Result.Failure<PermissionDto>(
                Error.Conflict("Permission.Duplicate", $"Permission '{request.Request.Name}' already exists"));
        }

        var permission = Permission.Create(request.Request.Name);

        await _dbContext.Permissions.AddAsync(permission, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new PermissionDto(permission.Id.Value, permission.Name));
    }
}
