using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Application.AuthUsers.Queries.GetCurrentUser;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly IUserManager<AuthUser> _userManager;

    public GetCurrentUserQueryHandler(
        IUserManager<AuthUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return Result.Failure<CurrentUserDto>(AuthUserErrors.NotFound);
        }

        var currentUserDTO = new CurrentUserDto(user.Id, user.Email!, user.UserName);

        return Result.Success(currentUserDTO);
    }
}