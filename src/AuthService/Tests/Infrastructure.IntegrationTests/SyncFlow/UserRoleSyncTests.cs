using Domain.AuthUsers;
using Domain.Roles;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Infrastructure.IntegrationTests.SyncFlow;

/// <summary>
/// Integration tests for User and Role synchronization to Identity tables.
/// These tests verify the real end-to-end flow using a real database.
/// </summary>
public class UserRoleSyncTests : IntegrationTestsBase
{
    [Fact]
    public async Task CreateAuthUser_ShouldSyncToIdentityUser()
    {
        // Arrange
        using var scope = CreateScope();
        var dbContext = GetWriteDbContext(scope);

        var roleResult = Role.Create("TestUser");
        var role = roleResult.Value;
        
        dbContext.DomainRoles.Add(role);
        await dbContext.SaveChangesAsync();

        var authUserResult = AuthUser.Create("test@example.com", "testuser", role);
        var authUser = authUserResult.Value;

        // Act
        dbContext.DomainUsers.Add(authUser);
        await dbContext.SaveChangesAsync();

        // Assert - Verify domain user was saved
        var savedAuthUser = await dbContext.DomainUsers
            .FirstOrDefaultAsync(u => u.Id == authUser.Id);
        savedAuthUser.Should().NotBeNull();
        savedAuthUser!.Email.Should().Be("test@example.com");

        // Assert - Verify Identity user was created through sync
        var identityUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == authUser.Id);
        identityUser.Should().NotBeNull();
        identityUser!.Email.Should().Be("test@example.com");
        identityUser.UserName.Should().Be("testuser");
    }

    [Fact]
    public async Task CreateRole_ShouldSyncToIdentityRole()
    {
        // Arrange
        using var scope = CreateScope();
        var dbContext = GetWriteDbContext(scope);
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationIdentityRole>>();

        var roleResult = Role.Create("AdminRole");
        var role = roleResult.Value;

        // Act
        dbContext.DomainRoles.Add(role);
        await dbContext.SaveChangesAsync();

        // Assert - Verify domain role was saved
        var savedRole = await dbContext.DomainRoles
            .FirstOrDefaultAsync(r => r.Id == role.Id);
        savedRole.Should().NotBeNull();
        savedRole!.Name.Should().Be("AdminRole");

        // Assert - Verify Identity role was created through sync
        var identityRole = await roleManager.FindByIdAsync(role.Id.ToString());
        identityRole.Should().NotBeNull();
        identityRole!.Name.Should().Be("AdminRole");
    }

    [Fact]
    public async Task AddRoleToUser_ShouldSyncToUserRoles()
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

        var adminRoleResult = Role.Create("Admin");
        var adminRole = adminRoleResult.Value;
        dbContext.DomainRoles.Add(adminRole);
        await dbContext.SaveChangesAsync();

        // Act - Add a new role to the user
        authUser.AddRole(adminRole);
        await dbContext.SaveChangesAsync();

        // Assert - Verify UserRole relationship exists in Identity tables
        var userRole = await dbContext.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == authUser.Id && ur.RoleId == adminRole.Id);
        userRole.Should().NotBeNull();
    }

    [Fact]
    public async Task RemoveRoleFromUser_ShouldRemoveFromUserRoles()
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

        // Verify the role exists
        var initialUserRole = await dbContext.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == authUser.Id && ur.RoleId == role.Id);
        initialUserRole.Should().NotBeNull();

        // Act - Remove the role from the user
        authUser.RemoveRole(role);
        await dbContext.SaveChangesAsync();

        // Assert - Verify UserRole relationship is removed
        var removedUserRole = await dbContext.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == authUser.Id && ur.RoleId == role.Id);
        removedUserRole.Should().BeNull();
    }

    [Fact]
    public async Task MultipleUsers_ShouldSyncIndependently()
    {
        // Arrange
        using var scope = CreateScope();
        var dbContext = GetWriteDbContext(scope);

        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        dbContext.DomainRoles.Add(role);
        await dbContext.SaveChangesAsync();

        var user1Result = AuthUser.Create("user1@example.com", "user1", role);
        var user1 = user1Result.Value;
        var user2Result = AuthUser.Create("user2@example.com", "user2", role);
        var user2 = user2Result.Value;

        // Act
        dbContext.DomainUsers.Add(user1);
        dbContext.DomainUsers.Add(user2);
        await dbContext.SaveChangesAsync();

        // Assert
        var identityUser1 = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == user1.Id);
        var identityUser2 = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == user2.Id);

        identityUser1.Should().NotBeNull();
        identityUser2.Should().NotBeNull();
        identityUser1!.Email.Should().Be("user1@example.com");
        identityUser2!.Email.Should().Be("user2@example.com");
        identityUser1.Id.Should().NotBe(identityUser2.Id);
    }
}
