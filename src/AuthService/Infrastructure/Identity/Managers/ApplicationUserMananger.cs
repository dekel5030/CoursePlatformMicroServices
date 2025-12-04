using Application.Abstractions.Identity;
using Application.Extensions;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Infrastructure.Identity.Extensions;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Managers;

public class ApplicationUserMananger : IUserManager<AuthUser>
{
    private readonly UserManager<ApplicationIdentityUser> _aspUserManager;

    public ApplicationUserMananger(UserManager<ApplicationIdentityUser> userManager)
    {
        _aspUserManager = userManager;
    }

    public async Task<Result> AddToRoleAsync(AuthUser domainUser, string defaultRole)
    {
        ApplicationIdentityUser? identityUser = await _aspUserManager.FindByIdAsync(domainUser.Id.ToString());

        if (identityUser == null)
        {
            return Result.Failure(AuthUserErrors.NotFound);
        }

        IdentityResult identityResult = await _aspUserManager.AddToRoleAsync(identityUser, defaultRole);

        return identityResult.ToApplicationResult();
    }

    public async Task<Result> CreateAsync(AuthUser user, string password)
    {
        ApplicationIdentityUser identityUser = new(user);

        IdentityResult result = await _aspUserManager.CreateAsync(identityUser, password);

        return result.ToApplicationResult();
    }

    public async Task<AuthUser?> FindByEmailAsync(string email)
    {
        ApplicationIdentityUser? user = await _aspUserManager.FindByEmailAsync(email);

        return user?.DomainUser ?? null;
    }

    public async Task<AuthUser?> FindByIdAsync(string userId)
    {
        ApplicationIdentityUser? identityUser = await _aspUserManager.FindByIdAsync(userId);
        return identityUser?.DomainUser ?? null;
    }
}