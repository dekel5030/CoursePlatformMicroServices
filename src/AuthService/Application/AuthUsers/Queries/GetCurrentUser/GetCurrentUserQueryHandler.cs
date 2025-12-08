using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Kernel.Auth.Abstractions;

namespace Application.AuthUsers.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly IUserManager<AuthUser> _userManager;
    private readonly IUserContext _userContext;

    public GetCurrentUserQueryHandler(
        IUserManager<AuthUser> userManager,
        IUserContext userContext)
    {
        _userManager = userManager;
        _userContext = userContext;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        Guid? userId = _userContext.UserId;

        if (userId is null)
        {
            return Result.Failure<CurrentUserDto>(AuthUserErrors.Unauthorized);
        }

        var user = await _userManager.FindByIdAsync(userId.Value);

        if (user is null)
        {
            return Result.Failure<CurrentUserDto>(AuthUserErrors.NotFound);
        }

        var currentUserDTO = new CurrentUserDto(user.Id, user.Email, user.UserName);

        return Result.Success(currentUserDTO);
    }
}