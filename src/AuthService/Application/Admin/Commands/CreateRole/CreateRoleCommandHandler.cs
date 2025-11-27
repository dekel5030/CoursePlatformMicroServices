using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Admin.Dtos;
using Domain.Roles;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Commands.CreateRole;

public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, RoleDto>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;

    public CreateRoleCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result<RoleDto>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken = default)
    {
        var existingRole = await _readDbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == request.Request.Name, cancellationToken);

        if (existingRole != null)
        {
            return Result.Failure<RoleDto>(
                Error.Conflict("Role.Duplicate", $"Role '{request.Request.Name}' already exists"));
        }

        var role = Role.Create(request.Request.Name);

        await _dbContext.Roles.AddAsync(role, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new RoleDto(role.Id.Value, role.Name, Enumerable.Empty<PermissionDto>()));
    }
}
