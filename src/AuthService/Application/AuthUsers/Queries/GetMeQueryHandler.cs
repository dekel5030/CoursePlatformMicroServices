using Auth.Application.Abstractions.Data;
using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Errors;
using Auth.Domain.AuthUsers.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.AuthUsers.Queries;

internal class GetMeQueryHandler : IQueryHandler<GetMeQuery, UserDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetMeQueryHandler(IReadDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<UserDto>> Handle(
        GetMeQuery request,
        CancellationToken cancellationToken = default)
    {
        if (!_userContext.IsAuthenticated || _userContext.Id is null)
        {
            return Result<UserDto>.Failure(AuthUserErrors.NotFound);
        }

        AuthUserId userId = new AuthUserId(_userContext.Id.Value);

        var user = await _dbContext.Users.Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Result<UserDto>.Failure(AuthUserErrors.NotFound);
        }

        var userDto = new UserDto(
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
        );

        return Result<UserDto>.Success(userDto);
    }
}
