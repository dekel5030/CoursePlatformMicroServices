using Domain.Roles;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.Roles;

/// <summary>
/// Unit tests for the Role domain entity.
/// Tests cover role creation, validation, and business invariants.
/// </summary>
public class RoleTests
{
    /// <summary>
    /// Verifies that Role.Create successfully creates a role with a valid name.
    /// </summary>
    [Fact]
    public void Create_WithValidName_ShouldCreateRoleSuccessfully()
    {
        // Arrange
        var roleName = "Administrator";

        // Act
        var role = Role.Create(roleName);

        // Assert
        role.Should().NotBeNull();
        role.Id.Should().NotBeEmpty();
        role.Name.Should().Be(roleName);
    }

    /// <summary>
    /// Verifies that different roles can be created with different names.
    /// </summary>
    [Theory]
    [InlineData("User")]
    [InlineData("Administrator")]
    [InlineData("Moderator")]
    [InlineData("Guest")]
    public void Create_WithDifferentNames_ShouldCreateDifferentRoles(string roleName)
    {
        // Arrange & Act
        var role = Role.Create(roleName);

        // Assert
        role.Name.Should().Be(roleName);
        role.Id.Should().NotBeEmpty();
    }

    /// <summary>
    /// Verifies that each created role has a unique GUID identifier.
    /// </summary>
    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        // Arrange & Act
        var role1 = Role.Create("User");
        var role2 = Role.Create("Admin");

        // Assert
        role1.Id.Should().NotBe(role2.Id);
    }

    /// <summary>
    /// Verifies that roles with the same name have different IDs (representing different instances).
    /// </summary>
    [Fact]
    public void Create_WithSameName_ShouldCreateRolesWithDifferentIds()
    {
        // Arrange
        var roleName = "User";

        // Act
        var role1 = Role.Create(roleName);
        var role2 = Role.Create(roleName);

        // Assert
        role1.Id.Should().NotBe(role2.Id);
        role1.Name.Should().Be(role2.Name);
    }

    /// <summary>
    /// Verifies that a role can be created with special characters in the name.
    /// </summary>
    [Theory]
    [InlineData("Super-Admin")]
    [InlineData("Content_Manager")]
    [InlineData("Read.Only.User")]
    public void Create_WithSpecialCharactersInName_ShouldCreateSuccessfully(string roleName)
    {
        // Arrange & Act
        var role = Role.Create(roleName);

        // Assert
        role.Name.Should().Be(roleName);
    }
}
