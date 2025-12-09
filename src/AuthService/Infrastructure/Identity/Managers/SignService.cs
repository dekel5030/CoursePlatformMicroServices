using Application.Abstractions.Identity;
using Application.Extensions;
using Domain.AuthUsers;
using Infrastructure.Database;
using Infrastructure.Identity.Extensions;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Managers;

public class SignService : ISignService<AuthUser>
{
    private readonly WriteDbContext _dbContext;
    private readonly SignInManager<ApplicationIdentityUser> _aspSignInManager;
    public SignService(
        SignInManager<ApplicationIdentityUser> signInManager,
        WriteDbContext dbContext)
    {
        _aspSignInManager = signInManager;
        _dbContext = dbContext;
    }

    public async Task<Result> RegisterAsync(AuthUser user, string password)
    {
        var identityUser = new ApplicationIdentityUser(user);
        IdentityResult creatingResult = await _aspSignInManager.UserManager.CreateAsync(identityUser, password);

        _dbContext.AuthUsers.Add(user);
        return creatingResult.ToApplicationResult();
    }

    public async Task<Result> PasswordSignInAsync(AuthUser user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        SignInResult result = await _aspSignInManager
            .PasswordSignInAsync(user.UserName, password, isPersistent, lockoutOnFailure);

        _dbContext.AuthUsers.Add(user);
        return result.ToApplicationResult();
    }

    public async Task SignInAsync(AuthUser user, bool isPersistent)
    {
        var identityUser = await _aspSignInManager.UserManager.FindByIdAsync(user.Id.ToString());

        if (identityUser == null)
        {
            throw new InvalidOperationException($"User with Id {user.Id} not found");
        }

        await _aspSignInManager.SignInAsync(identityUser, isPersistent);
    }

    public Task SignOutAsync()
    {
        return _aspSignInManager.SignOutAsync();
    }
}