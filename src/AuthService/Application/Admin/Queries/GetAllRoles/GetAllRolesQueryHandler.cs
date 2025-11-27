using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Admin.Dtos;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Queries.GetAllRoles;

public class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetAllRolesQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IEnumerable<RoleDto>>> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken = default)
    {
        var roles = await _readDbContext.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Select(r => new RoleDto(
                r.Id.Value,
                r.Name,
                r.RolePermissions.Select(rp => new PermissionDto(rp.Permission.Id.Value, rp.Permission.Name))))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<RoleDto>>(roles);
    }
}
