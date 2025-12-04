using Application.Abstractions.Identity;
using Domain.AuthUsers;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationSignInManager : ISignInManager<AuthUser>
{
    private readonly SignInManager<ApplicationIdentityUser> _aspSignInManager;
    private readonly IUserManager<AuthUser> _userManager;

    public ApplicationSignInManager(
        SignInManager<ApplicationIdentityUser> signInManager, 
        IUserManager<AuthUser> userManager)
    {
        _aspSignInManager = signInManager;
        _userManager = userManager;
    }

    public IUserManager<AuthUser> UserManager => _userManager;

    public async Task<Result> PasswordSignInAsync(AuthUser user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        SignInResult result = await _aspSignInManager
            .PasswordSignInAsync(user.UserName, password, isPersistent, lockoutOnFailure);

        return result.ToApplicationResult();
    }

    public async Task SignInAsync(AuthUser user, bool isPersistent)
    {
        var identityUser = await _aspSignInManager.UserManager.FindByIdAsync(user.Id.ToString());

        if (identityUser == null)
        {
            return;
        }

        await _aspSignInManager.SignInAsync(identityUser, isPersistent);
    }

    public Task SignOutAsync()
    {
        return _aspSignInManager.SignOutAsync();
    }
}