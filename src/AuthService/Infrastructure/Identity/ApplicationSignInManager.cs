using Application.Abstractions.Identity;
using Domain.AuthUsers;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationSignInManager : ISignInManager<AuthUser>
{
    private readonly SignInManager<IdentityUser> _aspSignInManager;

    public ApplicationSignInManager(SignInManager<IdentityUser> signInManager)
    {
        _aspSignInManager = signInManager;
    }

    public IUserManager<AuthUser> UserManager => throw new NotImplementedException();

    public async Task<Result> PasswordSignInAsync(AuthUser user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        SignInResult signInResult = await _aspSignInManager
            .PasswordSignInAsync(user.Id.ToString(), password, isPersistent, lockoutOnFailure);

        
    }

    public Task SignInAsync(AuthUser user, bool isPersistent)
    {
        throw new NotImplementedException();
    }

    public Task SignOutAsync()
    {
        throw new NotImplementedException();
    }
}
