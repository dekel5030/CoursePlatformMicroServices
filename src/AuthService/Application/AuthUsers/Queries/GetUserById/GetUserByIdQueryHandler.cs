using Auth.Application.Abstractions.Data;
using Auth.Application.AuthUsers.Queries.Dtos;
using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Errors;
using Auth.Domain.AuthUsers.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.AuthUsers.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IReadDbContext _dbContext;

    public GetUserByIdQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var userId = new AuthUserId(request.UserId);

        AuthUser? user = await _dbContext.Users.Include(u => u.Roles)
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
