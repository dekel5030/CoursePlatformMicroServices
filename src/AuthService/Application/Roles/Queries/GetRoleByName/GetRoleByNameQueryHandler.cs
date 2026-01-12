using Auth.Application.Abstractions.Data;
using Auth.Application.AuthUsers.Queries.Dtos;
using Auth.Domain.Roles;
using Auth.Domain.Roles.Errors;
using Auth.Domain.Roles.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Roles.Queries.GetRoleByName;

internal sealed class GetRoleByNameQueryHandler : IQueryHandler<GetRoleByNameQuery, RoleDto>
{
    private readonly IReadDbContext _dbContext;

    public GetRoleByNameQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<RoleDto>> Handle(
        GetRoleByNameQuery request, 
        CancellationToken cancellationToken = default)
    {
        var roleName = new RoleName(request.RoleName);

        Role? role = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);

        if (role == null)
        {
            return Result<RoleDto>.Failure(RoleErrors.NotFound);
        }

        var roleDto = new RoleDto(
            role.Id.Value, 
            role.Name.Value,
            role.Permissions
                .Select(permission => new PermissionDto(
                    permission.Key, 
                    permission.Effect.ToString(), 
                    permission.Action.ToString(),
                    permission.Resource.ToString(),
                    permission.ResourceId.Value))
                .ToList()
            );

        return Result.Success(roleDto);
    }
}
