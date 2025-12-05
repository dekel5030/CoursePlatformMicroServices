using Application.Abstractions.Identity;
using Application.Extensions;
using Domain.Roles;
using Infrastructure.Database;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Managers;

public class ApplicationRoleManger : IRoleManager<Role>
{
    private readonly RoleManager<IdentityRole<Guid>> _aspRoleManager;
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public IQueryable<Role> Roles => _readDbContext.Roles.AsQueryable();
    
    public ApplicationRoleManger(
        RoleManager<IdentityRole<Guid>> aspRoleManager, 
        WriteDbContext writeDbContext, 
        ReadDbContext readDbContext)
    {
        _aspRoleManager = aspRoleManager;
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }


    public async Task<Result> CreateAsync(Role role)
    {
        IdentityRole<Guid> aspRole = new IdentityRole<Guid>
        {
            Id = role.Id,
            Name = role.Name,
        };


        IdentityResult result = await _aspRoleManager.CreateAsync(aspRole);

        if (!result.Succeeded)
        {
            return result.ToApplicationResult();
        }

        await _writeDbContext.DomainRoles.AddAsync(role);
        await _writeDbContext.SaveChangesAsync();

        return result.ToApplicationResult();
    }
}
