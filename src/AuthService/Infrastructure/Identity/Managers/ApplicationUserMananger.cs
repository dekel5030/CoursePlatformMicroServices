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
    private readonly UserManager<IdentityUser<Guid>> _aspUserManager;
    private readonly WriteDbContext _writeDbContext;

    public ApplicationUserMananger(
        UserManager<IdentityUser<Guid>> userManager, 
        WriteDbContext writeDbContext)
    {
        _aspUserManager = userManager;
        _writeDbContext = writeDbContext;
    }

    public async Task<Result> AddToRoleAsync(AuthUser domainUser, string defaultRole)
    {
        IdentityUser<Guid>? identityUser = await _aspUserManager.FindByIdAsync(domainUser.Id.ToString());

        if (identityUser == null)
        {
            return Result.Failure(AuthUserErrors.NotFound);
        }

        IdentityResult identityResult = await _aspUserManager.AddToRoleAsync(identityUser, defaultRole);

        return identityResult.ToApplicationResult();
    }

    public async Task<Result> CreateAsync(AuthUser user, string password)
    {
        IdentityUser<Guid> identityUser = new() { 
            Id = user.Id, 
            UserName = user.UserName, 
            Email = user.Email};

        IdentityResult result = await _aspUserManager.CreateAsync(identityUser, password);

        if (!result.Succeeded)
        {
            return result.ToApplicationResult();
        }

        await _writeDbContext.DomainUsers.AddAsync(user);
        await _writeDbContext.SaveChangesAsync();
        return result.ToApplicationResult();
    }

    public Task<AuthUser?> FindByEmailAsync(string email)
    {
        return _writeDbContext.DomainUsers.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<AuthUser?> FindByIdAsync(Guid userId)
    {
        return await _writeDbContext.DomainUsers.FindAsync(userId);
    }
}