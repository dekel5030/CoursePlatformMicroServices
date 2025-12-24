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
        List<RoleDto> roles = await _readDbContext.Roles
            .Select(r => new RoleDto(r.Id.Value, r.Name.Value, r.Permissions.Count))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyCollection<RoleDto>>.Success(roles);
    }
}
