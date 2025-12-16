using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Application.AuthUsers.Queries.GetCurrentUser;
using Auth.Contracts.Dtos;
using Domain.AuthUsers.Errors;
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
        Guid? userId = _userContext.UserId;

        if (userId == null)
        {
            return Result.Failure<UserAuthDataDto>(AuthUserErrors.Unauthorized);
        }

        var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(
            u => u.Id == userId.Value,
            cancellationToken);

        var response = new CurrentUserDto(
            Id: user!.Id,
            Email: user.Email,
            UserName: user.UserName);

        return Result.Success(response);
    }
}