using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthUsers.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, AuthResponseDto>
{
    private readonly UserManager<AuthUser> _userManager;

    public GetCurrentUserQueryHandler(UserManager<AuthUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken = default)
    {
        var authUser = await _userManager.FindByEmailAsync(request.Email);

        if (authUser == null)
        {
            return Result.Failure<AuthResponseDto>(AuthUserErrors.NotFound);
        }

        // Extract roles and permissions using UserManager
        var roles = await _userManager.GetRolesAsync(authUser);
        var userClaims = (await _userManager.GetClaimsAsync(authUser)).Select(c => c.Value).ToList();

        var response = new AuthResponseDto
        {
            AuthUserId = authUser.Id,
            UserId = authUser.Id, // Unified ID
            Email = authUser.Email!,
            Roles = roles,
            AccessToken = null!, 
            Permissions = userClaims
        };

        return Result.Success(response);
    }
}
