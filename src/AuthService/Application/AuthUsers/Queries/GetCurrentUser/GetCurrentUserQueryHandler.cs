using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, AuthResponseDto>
{
    private readonly IReadDbContext _readDbContext;

    public GetCurrentUserQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken = default)
    {
        // Fetch user from database with all necessary navigation properties
        var authUser = await _readDbContext.AuthUsers
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (authUser == null)
        {
            return Result.Failure<AuthResponseDto>(AuthUserErrors.NotFound);
        }

        // Extract roles and permissions
        var roles = authUser.UserRoles
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var permissionsFromRoles = authUser.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name);

        var directPermissions = authUser.UserPermissions
            .Select(up => up.Permission.Name);

        var allPermissions = permissionsFromRoles
            .Concat(directPermissions)
            .Distinct()
            .ToList();

        var response = new AuthResponseDto
        {
            AuthUserId = authUser.Id.Value,
            UserId = authUser.Id.Value, // Unified ID
            Email = authUser.Email,
            Roles = roles,
            AccessToken = null!, 
            Permissions = allPermissions
        };

        return Result.Success(response);
    }
}
