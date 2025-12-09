using Domain.Roles;
using FluentAssertions;
using Infrastructure.Identity;
using Xunit;

namespace Infrastructure.UnitTests.Identity.Mappers;

/// <summary>
/// Unit tests for ApplicationIdentityRole mapping.
/// Verifies that domain Role entities are correctly mapped to ASP.NET Core Identity roles.
/// </summary>
public class ApplicationIdentityRoleTests
{
    [Fact]
    public void Constructor_WithDomainRole_ShouldMapIdCorrectly()
    {
        // Arrange
        var roleResult = Role.Create("TestRole");
        var role = roleResult.Value;

        // Act
        var identityRole = new ApplicationIdentityRole(role);

        // Assert
        identityRole.Id.Should().Be(role.Id);
    }

    [Fact]
    public void Constructor_WithDomainRole_ShouldMapNameCorrectly()
    {
        // Arrange
        var roleResult = Role.Create("AdminRole");
        var role = roleResult.Value;

        // Act
        var identityRole = new ApplicationIdentityRole(role);

        // Assert
        identityRole.Name.Should().Be(role.Name);
    }

    [Fact]
    public void Constructor_WithDomainRole_ShouldSetNormalizedNameCorrectly()
    {
        // Arrange
        var roleResult = Role.Create("UserRole");
        var role = roleResult.Value;

        // Act
        var identityRole = new ApplicationIdentityRole(role);

        // Assert
        identityRole.NormalizedName.Should().Be("USERROLE");
    }

    [Theory]
    [InlineData("Admin", "ADMIN")]
    [InlineData("User", "USER")]
    [InlineData("Moderator", "MODERATOR")]
    [InlineData("Guest", "GUEST")]
    public void Constructor_WithDifferentRoleNames_ShouldNormalizeCorrectly(string roleName, string expectedNormalized)
    {
        // Arrange
        var roleResult = Role.Create(roleName);
        var role = roleResult.Value;

        // Act
        var identityRole = new ApplicationIdentityRole(role);

        // Assert
        identityRole.Name.Should().Be(roleName);
        identityRole.NormalizedName.Should().Be(expectedNormalized);
    }

    [Fact]
    public void Constructor_ShouldPreserveGuidIdentity()
    {
        // Arrange
        var role1Result = Role.Create("Role1");
        var role1 = role1Result.Value;
        var role2Result = Role.Create("Role2");
        var role2 = role2Result.Value;

        // Act
        var identityRole1 = new ApplicationIdentityRole(role1);
        var identityRole2 = new ApplicationIdentityRole(role2);

        // Assert
        identityRole1.Id.Should().Be(role1.Id);
        identityRole2.Id.Should().Be(role2.Id);
        identityRole1.Id.Should().NotBe(identityRole2.Id);
    }

    [Fact]
    public void Constructor_ShouldInheritFromIdentityRoleWithGuid()
    {
        // Arrange
        var roleResult = Role.Create("TestRole");
        var role = roleResult.Value;

        // Act
        var identityRole = new ApplicationIdentityRole(role);

        // Assert
        identityRole.Should().BeAssignableTo<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>();
    }

    [Fact]
    public void Constructor_WithRoleNameContainingMixedCase_ShouldPreserveNameButNormalizeCorrectly()
    {
        // Arrange
        var roleResult = Role.Create("MixedCaseRole");
        var role = roleResult.Value;

        // Act
        var identityRole = new ApplicationIdentityRole(role);

        // Assert
        identityRole.Name.Should().Be("MixedCaseRole");
        identityRole.NormalizedName.Should().Be("MIXEDCASEROLE");
    }

    [Fact]
    public void Constructor_WithMultipleRoles_ShouldMaintainUniqueIds()
    {
        // Arrange
        var roles = new[]
        {
            Role.Create("Role1").Value,
            Role.Create("Role2").Value,
            Role.Create("Role3").Value
        };

        // Act
        var identityRoles = roles.Select(r => new ApplicationIdentityRole(r)).ToList();

        // Assert
        identityRoles.Should().HaveCount(3);
        identityRoles.Select(ir => ir.Id).Should().OnlyHaveUniqueItems();
        identityRoles.Select(ir => ir.Name).Should().OnlyHaveUniqueItems();
    }
}
