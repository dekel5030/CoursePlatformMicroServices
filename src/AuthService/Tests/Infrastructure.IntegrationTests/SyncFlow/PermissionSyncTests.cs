using Domain.AuthUsers;
using Domain.Permissions;
using Domain.Roles;
using FluentAssertions;
using Infrastructure.Database;
using Kernel.Auth.AuthTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Infrastructure.IntegrationTests.SyncFlow;

/// <summary>
/// Integration tests for Permission synchronization to Identity claims.
/// Verifies that domain permissions correctly sync to UserClaims and RoleClaims.
/// </summary>
public class PermissionSyncTests : IntegrationTestsBase
{
    [Fact]
    public async Task AddPermissionToUser_ShouldSyncToUserClaims()
    {
        // Arrange
        using var scope = CreateScope();
        var dbContext = GetWriteDbContext(scope);

        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        dbContext.DomainRoles.Add(role);
        await dbContext.SaveChangesAsync();

        var authUserResult = AuthUser.Create("test@example.com", "testuser", role);
        var authUser = authUserResult.Value;
        dbContext.DomainUsers.Add(authUser);
        await dbContext.SaveChangesAsync();

        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("*"));

        // Act - Add permission to user
        authUser.AddPermission(permission);
        await dbContext.SaveChangesAsync();

        // Assert - Verify permission claim exists in UserClaims
        var userClaim = await dbContext.UserClaims
            .FirstOrDefaultAsync(uc => 
                uc.UserId == authUser.Id && 
                uc.ClaimType == "cp_permission" &&
                uc.ClaimValue == "allow:read:course:*");
        
        userClaim.Should().NotBeNull();
    }

    [Fact]
    public async Task RemovePermissionFromUser_ShouldRemoveFromUserClaims()
    {
        // Arrange
        using var scope = CreateScope();
        var dbContext = GetWriteDbContext(scope);

        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        dbContext.DomainRoles.Add(role);
        await dbContext.SaveChangesAsync();

        var authUserResult = AuthUser.Create("test@example.com", "testuser", role);
        var authUser = authUserResult.Value;
        dbContext.DomainUsers.Add(authUser);
        await dbContext.SaveChangesAsync();

        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("*"));
        authUser.AddPermission(permission);
        await dbContext.SaveChangesAsync();

        // Verify claim exists
        var initialClaim = await dbContext.UserClaims
            .FirstOrDefaultAsync(uc => 
                uc.UserId == authUser.Id && 
                uc.ClaimType == "cp_permission" &&
                uc.ClaimValue == "allow:read:course:*");
        initialClaim.Should().NotBeNull();

        // Act - Remove permission from user
        authUser.RemovePermission(permission);
        await dbContext.SaveChangesAsync();

        // Assert - Verify permission claim is removed
        var removedClaim = await dbContext.UserClaims
            .FirstOrDefaultAsync(uc => 
                uc.UserId == authUser.Id && 
                uc.ClaimType == "cp_permission" &&
                uc.ClaimValue == "allow:read:course:*");
        
        removedClaim.Should().BeNull();
    }

    [Fact]
    public async Task AddPermissionToRole_ShouldSyncToRoleClaims()
    {
        // Arrange
        using var scope = CreateScope();
        var dbContext = GetWriteDbContext(scope);

        var roleResult = Role.Create("AdminRole");
        var role = roleResult.Value;
        dbContext.DomainRoles.Add(role);
        await dbContext.SaveChangesAsync();

        var permission = Permission.CreateForRole(
            ActionType.Update,
            ResourceType.User,
            ResourceId.Create("*"));

        // Act - Add permission to role
        role.AddPermission(permission);
        await dbContext.SaveChangesAsync();

        // Assert - Verify permission claim exists in RoleClaims
        var roleClaim = await dbContext.RoleClaims
            .FirstOrDefaultAsync(rc => 
                rc.RoleId == role.Id && 
                rc.ClaimType == "cp_permission" &&
                rc.ClaimValue == "allow:update:user:*");
        
        roleClaim.Should().NotBeNull();
    }

    [Fact]
    public async Task RemovePermissionFromRole_ShouldRemoveFromRoleClaims()
    {
        // Arrange
        using var scope = CreateScope();
        var dbContext = GetWriteDbContext(scope);

        var roleResult = Role.Create("AdminRole");
        var role = roleResult.Value;
        dbContext.DomainRoles.Add(role);
        await dbContext.SaveChangesAsync();

        var permission = Permission.CreateForRole(
            ActionType.Update,
            ResourceType.User,
            ResourceId.Create("*"));
        role.AddPermission(permission);
        await dbContext.SaveChangesAsync();

        // Verify claim exists
        var initialClaim = await dbContext.RoleClaims
            .FirstOrDefaultAsync(rc => 
                rc.RoleId == role.Id && 
                rc.ClaimType == "cp_permission" &&
                rc.ClaimValue == "allow:update:user:*");
        initialClaim.Should().NotBeNull();

        // Act - Remove permission from role
        role.RemovePermission(permission);
        await dbContext.SaveChangesAsync();

        // Assert - Verify permission claim is removed
        var removedClaim = await dbContext.RoleClaims
            .FirstOrDefaultAsync(rc => 
                rc.RoleId == role.Id && 
                rc.ClaimType == "cp_permission" &&
                rc.ClaimValue == "allow:update:user:*");
        
        removedClaim.Should().BeNull();
    }

    [Theory]
    [InlineData(ActionType.Create, ResourceType.Course, "123", "allow:create:course:123")]
    [InlineData(ActionType.Read, ResourceType.User, "*", "allow:read:user:*")]
    [InlineData(ActionType.Update, ResourceType.Enrollment, "456", "allow:update:enrollment:456")]
    [InlineData(ActionType.Delete, ResourceType.Lesson, "*", "allow:delete:lesson:*")]
    public async Task AddDifferentPermissionsToUser_ShouldCreateCorrectClaims(
        ActionType action, ResourceType resource, string resourceId, string expectedClaimValue)
    {
        // Arrange
        using var scope = CreateScope();
        var dbContext = GetWriteDbContext(scope);

        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        dbContext.DomainRoles.Add(role);
        await dbContext.SaveChangesAsync();

        var authUserResult = AuthUser.Create("test@example.com", "testuser", role);
        var authUser = authUserResult.Value;
        dbContext.DomainUsers.Add(authUser);
        await dbContext.SaveChangesAsync();

        var permission = Permission.CreateForRole(action, resource, ResourceId.Create(resourceId));

        // Act
        authUser.AddPermission(permission);
        await dbContext.SaveChangesAsync();

        // Assert
        var userClaim = await dbContext.UserClaims
            .FirstOrDefaultAsync(uc => 
                uc.UserId == authUser.Id && 
                uc.ClaimType == "cp_permission" &&
                uc.ClaimValue == expectedClaimValue);
        
        userClaim.Should().NotBeNull();
    }

    [Fact]
    public async Task AddMultiplePermissionsToUser_ShouldCreateMultipleClaims()
    {
        // Arrange
        using var scope = CreateScope();
        var dbContext = GetWriteDbContext(scope);

        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        dbContext.DomainRoles.Add(role);
        await dbContext.SaveChangesAsync();

        var authUserResult = AuthUser.Create("test@example.com", "testuser", role);
        var authUser = authUserResult.Value;
        dbContext.DomainUsers.Add(authUser);
        await dbContext.SaveChangesAsync();

        var permission1 = Permission.CreateForRole(ActionType.Read, ResourceType.Course, ResourceId.Create("*"));
        var permission2 = Permission.CreateForRole(ActionType.Update, ResourceType.Course, ResourceId.Create("123"));
        var permission3 = Permission.CreateForRole(ActionType.Delete, ResourceType.Lesson, ResourceId.Create("*"));

        // Act
        authUser.AddPermission(permission1);
        authUser.AddPermission(permission2);
        authUser.AddPermission(permission3);
        await dbContext.SaveChangesAsync();

        // Assert
        var userClaims = await dbContext.UserClaims
            .Where(uc => uc.UserId == authUser.Id && uc.ClaimType == "cp_permission")
            .ToListAsync();
        
        userClaims.Should().HaveCount(3);
        userClaims.Should().Contain(uc => uc.ClaimValue == "allow:read:course:*");
        userClaims.Should().Contain(uc => uc.ClaimValue == "allow:update:course:123");
        userClaims.Should().Contain(uc => uc.ClaimValue == "allow:delete:lesson:*");
    }
}
