using Application.Abstractions.Identity;
using Application.Extensions;
using Domain.Roles;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Managers;

public class ApplicationRoleManger : IRoleManager<Role>
{
    RoleManager<IdentityRole<Guid>> _aspRoleManager;

    public ApplicationRoleManger(RoleManager<IdentityRole<Guid>> aspRoleManager)
    {
        _aspRoleManager = aspRoleManager;
    }

    public async Task<Result> CreateAsync(Role role)
    {
        IdentityRole<Guid> aspRole = new IdentityRole<Guid>
        {
            Id = role.Id,
            Name = role.Name,
        };


        IdentityResult result = await _aspRoleManager.CreateAsync(aspRole);
        return result.ToApplicationResult();
    }
}
