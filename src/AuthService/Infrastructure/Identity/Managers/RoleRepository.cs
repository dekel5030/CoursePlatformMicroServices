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

    public async Task<Result> AddAsync(Role role, CancellationToken cancellationToken)
    {
        var identityRole = new ApplicationIdentityRole(role);

        IdentityResult identityResult = await _aspRoleManager.CreateAsync(identityRole);
        if (!identityResult.Succeeded)
            return identityResult.ToApplicationResult();

        foreach (var permission in role.Permissions)
        {
            var claim = new IdentityRoleClaim<Guid>
            {
                RoleId = identityRole.Id,
                ClaimType = PermissionClaim.ClaimType,
                ClaimValue = PermissionClaim.Create(
                    permission.Effect,
                    permission.Action,
                    permission.Resource,
                    permission.ResourceId).Value
            };

            _writeDbContext.RoleClaims.Add(claim);
        }

        _writeDbContext.DomainRoles.Add(role);
        return Result.Success();
    }


    public async Task<Result> UpdateAsync(Role role, CancellationToken cancellationToken)
    {
        var identityRole = await _aspRoleManager.FindByIdAsync(role.Id.ToString());
        if (identityRole is null)
            return Result.Failure(RoleErrors.NotFound);

        if (identityRole.Name != role.Name)
        {
            identityRole.Name = role.Name;
            identityRole.NormalizedName = _aspRoleManager.NormalizeKey(identityRole.Name);

            var updateResult = await _aspRoleManager.UpdateAsync(identityRole);
            if (!updateResult.Succeeded)
                return updateResult.ToApplicationResult();
        }

        await SyncClaimsAsync(identityRole, role);

        var domainRole = await _writeDbContext.DomainRoles
            .FirstOrDefaultAsync(r => r.Id == role.Id, cancellationToken);

        if (domainRole is null)
            return Result.Failure(RoleErrors.NotFound);

        _writeDbContext.Entry(domainRole).CurrentValues.SetValues(role);

        return Result.Success();
    }

    private async Task<Result> SyncClaimsAsync(
        ApplicationIdentityRole aspRole,
        Role role)
    {
        var identityClaims = await _aspRoleManager.GetClaimsAsync(aspRole);

        var identityPermissions = identityClaims
            .Where(c => c.Type == PermissionClaim.ClaimType)
            .ToDictionary(c => c.Value);

        var domainPermissions = role.Permissions
            .Select(p => PermissionClaim.Create(
                p.Effect, p.Action, p.Resource, p.ResourceId))
            .ToDictionary(c => c.Value);

        foreach (var claim in identityPermissions.Values.Except(domainPermissions.Values))
        {
            var removeResult = await _aspRoleManager.RemoveClaimAsync(aspRole, claim);
            if (!removeResult.Succeeded)
                return removeResult.ToApplicationResult();
        }

        foreach (var claim in domainPermissions.Values.Except(identityPermissions.Values))
        {
            var addResult = await _aspRoleManager.AddClaimAsync(aspRole, claim);
            if (!addResult.Succeeded)
                return addResult.ToApplicationResult();
        }

        return Result.Success();
    }


    public Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return _readDbContext.Roles.FindAsync(roleId, cancellationToken).AsTask();
    }
}
