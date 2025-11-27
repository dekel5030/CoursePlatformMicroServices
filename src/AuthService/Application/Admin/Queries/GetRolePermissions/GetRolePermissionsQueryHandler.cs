using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Admin.Dtos;
using Domain.Roles.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Queries.GetRolePermissions;

public class GetRolePermissionsQueryHandler : IQueryHandler<GetRolePermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetRolePermissionsQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IEnumerable<PermissionDto>>> Handle(
        GetRolePermissionsQuery request,
        CancellationToken cancellationToken = default)
    {
        var role = await _readDbContext.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == new RoleId(request.RoleId), cancellationToken);

        if (role == null)
        {
            return Result.Failure<IEnumerable<PermissionDto>>(
                Error.NotFound("Role.NotFound", $"Role with ID {request.RoleId} not found"));
        }

        var permissions = role.RolePermissions
            .Select(rp => new PermissionDto(rp.Permission.Id.Value, rp.Permission.Name));

        return Result.Success(permissions);
    }
}
