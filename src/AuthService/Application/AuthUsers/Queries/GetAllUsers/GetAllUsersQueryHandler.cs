using Auth.Application.Abstractions.Data;
using Auth.Application.AuthUsers.Queries.Dtos;
using Auth.Domain.AuthUsers;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.AuthUsers.Queries.GetAllUsers;

internal sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IReadDbContext _dbContext;

    public GetAllUsersQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken = default)
    {
        List<AuthUser> users = await _dbContext.Users
            .Include(u => u.Roles)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        var userDtos = users.Select(user => new UserDto(
            Id: user.Id.Value,
            Email: user.Email.Address,
            FirstName: user.FullName.FirstName.Name,
            LastName: user.FullName.LastName.Name,
            Roles: user.Roles.Select(role => new RoleDto(
                Id: role.Id.Value,
                Name: role.Name.Value
            )).ToList(),
            Permissions: user.Permissions.Select(p => new PermissionDto(
                Key: p.Key,
                Effect: p.Effect.ToString(),
                Action: p.Action.ToString(),
                Resource: p.Resource.ToString(),
                ResourceId: p.ResourceId.Value
            )).ToList()
        )).ToList();

        return Result<IReadOnlyList<UserDto>>.Success(userDtos);
    }
}
