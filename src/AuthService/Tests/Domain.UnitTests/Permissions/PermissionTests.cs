using Domain.Permissions;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.Permissions;

/// <summary>
/// Unit tests for the Permission domain entity.
/// Tests cover permission creation and validation.
/// </summary>
public class PermissionTests
{
    /// <summary>
    /// Verifies that Permission.Create successfully creates a permission with a valid name.
    /// </summary>
    [Fact]
    public void Create_WithValidName_ShouldCreatePermissionSuccessfully()
    {
        // Arrange
        var permissionName = "users:read";

        // Act
        var permission = Permission.Create(permissionName);

        // Assert
        permission.Should().NotBeNull();
        permission.Name.Should().Be(permissionName);
    }

    /// <summary>
    /// Verifies that different permissions can be created with different names.
    /// </summary>
    [Theory]
    [InlineData("users:read")]
    [InlineData("users:write")]
    [InlineData("users:delete")]
    [InlineData("courses:manage")]
    [InlineData("admin:all")]
    public void Create_WithDifferentNames_ShouldCreateDifferentPermissions(string permissionName)
    {
        // Arrange & Act
        var permission = Permission.Create(permissionName);

        // Assert
        permission.Name.Should().Be(permissionName);
    }

    /// <summary>
    /// Verifies that permissions can be created with various naming conventions.
    /// </summary>
    [Theory]
    [InlineData("read_users")]
    [InlineData("write-courses")]
    [InlineData("delete.enrollments")]
    [InlineData("ADMIN_ACCESS")]
    public void Create_WithVariousNamingConventions_ShouldCreateSuccessfully(string permissionName)
    {
        // Arrange & Act
        var permission = Permission.Create(permissionName);

        // Assert
        permission.Name.Should().Be(permissionName);
    }
}
