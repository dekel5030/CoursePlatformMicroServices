using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Admin.Dtos;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetAllUsersQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IEnumerable<UserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken = default)
    {
        var users = await _readDbContext.AuthUsers
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
            .Select(u => new UserDto(
                u.Id.Value,
                u.Email,
                u.IsActive,
                u.UserRoles.Select(ur => ur.Role.Name),
                u.UserPermissions.Select(up => up.Permission.Name)))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<UserDto>>(users);
    }
}
