using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers.Errors;
using Kernel;
using Kernel.Auth.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly IUserContext _userContext;
    private readonly IReadDbContext _dbContext;

    public GetCurrentUserQueryHandler(
        IUserContext userContext,
        IReadDbContext dbContext)
    {
        _userContext = userContext;
        _dbContext = dbContext;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        Guid? userId = _userContext.UserId;

        if (userId == null)
        {
            return Result.Failure<CurrentUserDto>(AuthUserErrors.Unauthorized);
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