using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Auth.Contracts.Dtos;
using Domain.AuthUsers.Errors;
using Domain.AuthUsers.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Queries.GetUserAuthData;

public class GetUserAuthDataQueryHandler : IQueryHandler<GetUserAuthDataQuery, UserAuthDataDto>
{
    private readonly IUserContext _userContext;
    private readonly IReadDbContext _dbContext;

    public GetUserAuthDataQueryHandler(
        IUserContext userContext,
        IReadDbContext dbContext)
    {
        _userContext = userContext;
        _dbContext = dbContext;
    }

    public async Task<Result<UserAuthDataDto>> Handle(
        GetUserAuthDataQuery request,
        CancellationToken cancellationToken)
    {
        Guid? userId = _userContext.Id;

        if (userId == null)
        {
            return Result.Failure<UserAuthDataDto>(AuthUserErrors.Unauthorized);
        }

        var authUserId = new AuthUserId(userId.Value);
        var user = await _dbContext.Users
            .Include(user => user.Permissions)
            .Include(user => user.Roles)
            .ThenInclude(role => role.Permissions)
            .FirstOrDefaultAsync(
                user => user.Id == authUserId,
                cancellationToken);

        if (user == null)
        {
            return Result.Failure<UserAuthDataDto>(AuthUserErrors.NotFound);
        }

        var distinctPermissions = user.Permissions
        .Concat(user.Roles.SelectMany(r => r.Permissions))
        .Distinct() 
        .Select(p => p.ToString())
        .ToList();

        var response = new UserAuthDataDto(
            user.Id.Value.ToString(),
            distinctPermissions,
            user.Roles.Select(role => role.Name).ToList());

        return Result.Success(response);
    }
}