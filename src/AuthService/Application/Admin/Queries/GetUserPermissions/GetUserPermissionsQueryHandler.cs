using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Admin.Dtos;
using Domain.AuthUsers.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler : IQueryHandler<GetUserPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetUserPermissionsQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IEnumerable<PermissionDto>>> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken = default)
    {
        var user = await _readDbContext.AuthUsers
            .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Id == new AuthUserId(request.UserId), cancellationToken);

        if (user == null)
        {
            return Result.Failure<IEnumerable<PermissionDto>>(
                Error.NotFound("User.NotFound", $"User with ID {request.UserId} not found"));
        }

        var permissions = user.UserPermissions
            .Select(up => new PermissionDto(up.Permission.Id.Value, up.Permission.Name));

        return Result.Success(permissions);
    }
}
