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
    private readonly IUserContext _userContext;

    public GetCurrentUserQueryHandler(
        IUserContext userContext)
    {
        _userContext = userContext;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        Guid? userId = _userContext.UserId;

        return Result.Success(new CurrentUserDto(Guid.Empty, ""));
    }
}