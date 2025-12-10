using Domain.AuthUsers;
using Domain.Roles;
using FluentAssertions;
using Infrastructure.Identity;
using Xunit;

namespace Infrastructure.UnitTests.Identity.Mappers;

/// <summary>
/// Unit tests for ApplicationIdentityUser mapping.
/// Verifies that domain AuthUser entities are correctly mapped to ASP.NET Core Identity users.
/// </summary>
public class ApplicationIdentityUserTests
{
    [Fact]
    public void Constructor_WithAuthUser_ShouldMapIdCorrectly()
    {
        // Arrange
        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        var authUser = AuthUser.Create("test@example.com", "testuser", role).Value;

        // Act
        var identityUser = new ApplicationIdentityUser(authUser);

        // Assert
        identityUser.Id.Should().Be(authUser.Id);
    }

    [Fact]
    public void Constructor_WithAuthUser_ShouldMapUserNameCorrectly()
    {
        // Arrange
        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        var authUser = AuthUser.Create("test@example.com", "testuser", role).Value;

        // Act
        var identityUser = new ApplicationIdentityUser(authUser);

        // Assert
        identityUser.UserName.Should().Be(authUser.UserName);
    }

    [Fact]
    public void Constructor_WithAuthUser_ShouldMapEmailCorrectly()
    {
        // Arrange
        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        var authUser = AuthUser.Create("admin@example.com", "admin", role).Value;

        // Act
        var identityUser = new ApplicationIdentityUser(authUser);

        // Assert
        identityUser.Email.Should().Be(authUser.Email);
    }

    [Theory]
    [InlineData("user1@example.com", "user1")]
    [InlineData("admin@example.com", "administrator")]
    [InlineData("moderator@test.com", "mod")]
    public void Constructor_WithDifferentAuthUsers_ShouldMapAllFieldsCorrectly(string email, string userName)
    {
        // Arrange
        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        var authUser = AuthUser.Create(email, userName, role).Value;

        // Act
        var identityUser = new ApplicationIdentityUser(authUser);

        // Assert
        identityUser.Id.Should().Be(authUser.Id);
        identityUser.UserName.Should().Be(userName);
        identityUser.Email.Should().Be(email);
    }

    [Fact]
    public void Constructor_WithAuthUserHavingEmptyUsername_ShouldMapEmailAsUsername()
    {
        // Arrange
        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        var authUser = AuthUser.Create("test@example.com", string.Empty, role).Value;

        // Act
        var identityUser = new ApplicationIdentityUser(authUser);

        // Assert
        identityUser.UserName.Should().Be(authUser.Email);
        identityUser.Email.Should().Be(authUser.Email);
    }

    [Fact]
    public void Constructor_ShouldPreserveGuidIdentity()
    {
        // Arrange
        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        var authUser1 = AuthUser.Create("user1@example.com", "user1", role).Value;
        var authUser2 = AuthUser.Create("user2@example.com", "user2", role).Value;

        // Act
        var identityUser1 = new ApplicationIdentityUser(authUser1);
        var identityUser2 = new ApplicationIdentityUser(authUser2);

        // Assert
        identityUser1.Id.Should().Be(authUser1.Id);
        identityUser2.Id.Should().Be(authUser2.Id);
        identityUser1.Id.Should().NotBe(identityUser2.Id);
    }

    [Fact]
    public void Constructor_ShouldInheritFromIdentityUserWithGuid()
    {
        // Arrange
        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        var authUser = AuthUser.Create("test@example.com", "testuser", role).Value;

        // Act
        var identityUser = new ApplicationIdentityUser(authUser);

        // Assert
        identityUser.Should().BeAssignableTo<Microsoft.AspNetCore.Identity.IdentityUser<Guid>>();
    }
}
