using Auth.Application.Abstractions.Data;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Roles.Queries.GetAllRoles;

internal class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, IReadOnlyCollection<RoleDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetAllRolesQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IReadOnlyCollection<RoleDto>>> Handle(
        GetAllRolesQuery request, 
        CancellationToken cancellationToken = default)
    {
        var roleUserCounts = await _readDbContext.Users
            .SelectMany(u => u.Roles.Select(r => r.Id))
            .GroupBy(roleId => roleId)
            .Select(g => new { RoleId = g.Key, UserCount = g.Count() })
            .ToListAsync(cancellationToken);

        var roles = await _readDbContext.Roles.ToListAsync(cancellationToken);

        List<RoleDto> roleDtos = roles.Select(role => new RoleDto(
            role.Id.Value,
            role.Name.Value,
            role.Permissions.Count,
            roleUserCounts.FirstOrDefault(ruc => ruc.RoleId == role.Id)?.UserCount ?? 0
        )).ToList();

        return Result<IReadOnlyCollection<RoleDto>>.Success(roleDtos);
    }
}
