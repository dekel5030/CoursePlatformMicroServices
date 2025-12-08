using System.Security.Claims;
using Application.Abstractions.Identity;
using Application.Extensions;
using Domain.Roles;
using Domain.Roles.Errors;
using Infrastructure.Database;
using Kernel;
using Kernel.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Managers;

public class RoleRepository : IRoleRepository<Role>
{
    private readonly RoleManager<ApplicationIdentityRole> _aspRoleManager;
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public IQueryable<Role> Roles => _readDbContext.Roles.AsNoTracking();
    
    public RoleRepository(
        RoleManager<ApplicationIdentityRole> aspRoleManager, 
        WriteDbContext writeDbContext, 
        ReadDbContext readDbContext)
    {
        _aspRoleManager = aspRoleManager;
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }

    public async Task<Result> AddRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        ApplicationIdentityRole aspRole = new ApplicationIdentityRole(role);
        IdentityResult result = await _aspRoleManager.CreateAsync(aspRole);

        if (!result.Succeeded)
        {
            return result.ToApplicationResult();
        }

        foreach (var permission in role.Permissions)
        {
            var permissionClaim = PermissionClaim.Create(
                    permission.Effect,
                    permission.Action,
                    permission.Resource,
                    permission.ResourceId);

            IdentityResult claimResult = await _aspRoleManager.AddClaimAsync(aspRole, permissionClaim);

            if (!claimResult.Succeeded)
            {
                return claimResult.ToApplicationResult();
            }
        }

        return result.ToApplicationResult();
    }

    public async Task<Result> UpdateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        var aspRole = await _writeDbContext.Roles
            .Include(r => r.DomainRole)
            .Include(r => r.Claims)
            .Include(r => r.DomainRole.Permissions)
            .FirstOrDefaultAsync(r => r.Id == role.Id, cancellationToken);

        if (aspRole is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        if (aspRole.Name != role.Name)
        {
            aspRole.Name = role.Name;
            var updateResult = await _aspRoleManager.UpdateAsync(aspRole);
            if (!updateResult.Succeeded) return updateResult.ToApplicationResult();
        }

        var currentClaims = await _aspRoleManager.GetClaimsAsync(aspRole);
        List<Claim> currentPermissionClaims = currentClaims
            .Where(c => c.Type == PermissionClaim.ClaimType).ToList();


        List<Claim> newPermissionClaims = role.Permissions.Select(p =>
            PermissionClaim.Create(p.Effect, p.Action, p.Resource, p.ResourceId))
            .ToList();

        List<Claim> claimsToAdd = newPermissionClaims
            .Where(newClaim => !currentPermissionClaims.Any(existing => existing.Value == newClaim.Value))
            .ToList();

        List<Claim> claimsToRemove = currentPermissionClaims
            .Where(existing => !newPermissionClaims.Any(newClaim => newClaim.Value == existing.Value))
            .ToList();

        foreach (var claim in claimsToRemove)
        {
            var removeResult = await _aspRoleManager.RemoveClaimAsync(aspRole, claim);
            if (!removeResult.Succeeded) return removeResult.ToApplicationResult();
        }

        foreach (var claim in claimsToAdd)
        {
            var addResult = await _aspRoleManager.AddClaimAsync(aspRole, claim);
            if (!addResult.Succeeded) return addResult.ToApplicationResult();
        }

        _writeDbContext.Entry(aspRole.DomainRole).CurrentValues.SetValues(role);

        return Result.Success();
    }

    public Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return _readDbContext
            .Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }
}
