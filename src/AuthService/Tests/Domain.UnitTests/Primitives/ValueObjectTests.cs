using Domain.AuthUsers.Primitives;
using Domain.Permissions.Primitives;
using Domain.Roles.Primitives;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.Primitives;

/// <summary>
/// Unit tests for value objects (primitives) used in the domain.
/// Tests verify value object equality, immutability, and proper behavior.
/// </summary>
public class ValueObjectTests
{
    #region AuthUserId Tests

    /// <summary>
    /// Verifies that AuthUserId value objects with the same GUID are equal.
    /// </summary>
    [Fact]
    public void AuthUserId_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var authUserId1 = new AuthUserId(guid);
        var authUserId2 = new AuthUserId(guid);

        // Act & Assert
        authUserId1.Should().Be(authUserId2);
        authUserId1.Value.Should().Be(authUserId2.Value);
    }

    /// <summary>
    /// Verifies that AuthUserId value objects with different GUIDs are not equal.
    /// </summary>
    [Fact]
    public void AuthUserId_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var authUserId1 = new AuthUserId(Guid.NewGuid());
        var authUserId2 = new AuthUserId(Guid.NewGuid());

        // Act & Assert
        authUserId1.Should().NotBe(authUserId2);
    }

    /// <summary>
    /// Verifies that AuthUserId properly stores the GUID value.
    /// </summary>
    [Fact]
    public void AuthUserId_ShouldStoreGuidValue()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();

        // Act
        var authUserId = new AuthUserId(expectedGuid);

        // Assert
        authUserId.Value.Should().Be(expectedGuid);
    }

    #endregion

    #region RoleId Tests

    /// <summary>
    /// Verifies that RoleId value objects with the same integer are equal.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public void RoleId_WithSameValue_ShouldBeEqual(int value)
    {
        // Arrange
        var roleId1 = new RoleId(value);
        var roleId2 = new RoleId(value);

        // Act & Assert
        roleId1.Should().Be(roleId2);
        roleId1.Value.Should().Be(roleId2.Value);
    }

    /// <summary>
    /// Verifies that RoleId value objects with different integers are not equal.
    /// </summary>
    [Fact]
    public void RoleId_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var roleId1 = new RoleId(1);
        var roleId2 = new RoleId(2);

        // Act & Assert
        roleId1.Should().NotBe(roleId2);
    }

    /// <summary>
    /// Verifies that RoleId properly stores the integer value.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(1000)]
    public void RoleId_ShouldStoreIntValue(int expectedValue)
    {
        // Arrange & Act
        var roleId = new RoleId(expectedValue);

        // Assert
        roleId.Value.Should().Be(expectedValue);
    }

    #endregion

    #region PermissionId Tests

    /// <summary>
    /// Verifies that PermissionId value objects with the same integer are equal.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(999)]
    public void PermissionId_WithSameValue_ShouldBeEqual(int value)
    {
        // Arrange
        var permissionId1 = new PermissionId(value);
        var permissionId2 = new PermissionId(value);

        // Act & Assert
        permissionId1.Should().Be(permissionId2);
        permissionId1.Value.Should().Be(permissionId2.Value);
    }

    /// <summary>
    /// Verifies that PermissionId value objects with different integers are not equal.
    /// </summary>
    [Fact]
    public void PermissionId_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var permissionId1 = new PermissionId(1);
        var permissionId2 = new PermissionId(2);

        // Act & Assert
        permissionId1.Should().NotBe(permissionId2);
    }

    /// <summary>
    /// Verifies that PermissionId properly stores the integer value.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void PermissionId_ShouldStoreIntValue(int expectedValue)
    {
        // Arrange & Act
        var permissionId = new PermissionId(expectedValue);

        // Assert
        permissionId.Value.Should().Be(expectedValue);
    }

    #endregion
}
