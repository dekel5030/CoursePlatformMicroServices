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
        var roles = await _readDbContext.Roles.ToListAsync(cancellationToken);
        var users = await _readDbContext.Users.Include(u => u.Roles).ToListAsync(cancellationToken);

        List<RoleDto> roleDtos = roles.Select(role => new RoleDto(
            role.Id.Value,
            role.Name.Value,
            role.Permissions.Count,
            users.Count(u => u.Roles.Any(r => r.Id == role.Id))
        )).ToList();

        return Result<IReadOnlyCollection<RoleDto>>.Success(roleDtos);
    }
}
