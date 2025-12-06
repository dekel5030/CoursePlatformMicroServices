using Application.Abstractions.Identity;
using Application.Extensions;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Infrastructure.Database;
using Infrastructure.Identity.Extensions;
using Kernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Managers;

public class ApplicationUserMananger : IUserManager<AuthUser>
{
    private readonly UserManager<ApplicationIdentityUser> _aspUserManager;
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public ApplicationUserMananger(
        UserManager<ApplicationIdentityUser> userManager,
        WriteDbContext writeDbContext,
        ReadDbContext readDbContext)
    {
        _aspUserManager = userManager;
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
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

    public async Task<Result> AddUserAsync(AuthUser user, string password)
    {
        var identityUser = new ApplicationIdentityUser(user);

        foreach (var role in user.Roles)
        {
            identityUser.UserRoles.Add(
                new IdentityUserRole<Guid>() { UserId = identityUser.Id, RoleId = role.Id });
        }

        IdentityResult result = await _aspUserManager.CreateAsync(identityUser, password);
        return result.ToApplicationResult();
    }

    public Task<AuthUser?> FindByEmailAsync(string email)
    {
        return _readDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<AuthUser?> FindByIdAsync(Guid userId)
    {
        return await _readDbContext.Users.FindAsync(userId);
    }
}