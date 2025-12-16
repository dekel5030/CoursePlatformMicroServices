using Domain.Permissions;
using Domain.Permissions.Errors;
using Domain.Roles;
using Domain.Roles.Errors;
using Domain.Roles.Events;
using FluentAssertions;
using Kernel.Auth.AuthTypes;
using Xunit;

namespace Domain.UnitTests.Roles;

/// <summary>
/// Unit tests for Role domain entity.
/// Tests cover role creation, permission management, and domain events.
/// </summary>
public class RoleTests
{
    #region Create Tests

    /// <summary>
    /// Verifies that Create successfully creates a Role with valid name.
    /// </summary>
    [Fact]
    public void Create_WithValidName_ShouldReturnSuccess()
    {
        // Arrange
        var roleName = "Admin";

        // Act
        var result = Role.Create(roleName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(roleName);
        result.Value.Id.Should().NotBeEmpty();
    }

    /// <summary>
    /// Verifies that Create returns failure when name is empty.
    /// </summary>
    [Fact]
    public void Create_WithEmptyName_ShouldReturnFailure()
    {
        // Arrange
        var roleName = string.Empty;

        // Act
        var result = Role.Create(roleName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoleErrors.NameCannotBeEmpty);
    }

    /// <summary>
    /// Verifies that Create returns failure when name is null.
    /// </summary>
    [Fact]
    public void Create_WithNullName_ShouldReturnFailure()
    {
        // Arrange
        string roleName = null!;

        // Act
        var result = Role.Create(roleName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoleErrors.NameCannotBeEmpty);
    }

    /// <summary>
    /// Verifies that Create returns failure when name is whitespace.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceName_ShouldReturnFailure()
    {
        // Arrange
        var roleName = "   ";

        // Act
        var result = Role.Create(roleName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoleErrors.NameCannotBeEmpty);
    }

    /// <summary>
    /// Verifies that Create raises RoleCreatedDomainEvent.
    /// </summary>
    [Fact]
    public void Create_ShouldRaiseRoleCreatedDomainEvent()
    {
        // Arrange
        var roleName = "Admin";

        // Act
        var result = Role.Create(roleName);

        // Assert
        result.Value.DomainEvents.Should().ContainSingle(e => e is RoleCreatedDomainEvent);
        var domainEvent = result.Value.DomainEvents.OfType<RoleCreatedDomainEvent>().First();
        domainEvent.Role.Should().Be(result.Value);
    }

    /// <summary>
    /// Verifies that Create generates unique IDs for different roles.
    /// </summary>
    [Fact]
    public void Create_ShouldGenerateUniqueId()
    {
        // Arrange & Act
        var role1 = Role.Create("Admin").Value;
        var role2 = Role.Create("User").Value;

        // Assert
        role1.Id.Should().NotBe(role2.Id);
    }

    /// <summary>
    /// Verifies that Create accepts various role names.
    /// </summary>
    [Theory]
    [InlineData("Admin")]
    [InlineData("User")]
    [InlineData("Moderator")]
    [InlineData("SuperAdmin")]
    public void Create_WithDifferentNames_ShouldCreateSuccessfully(string roleName)
    {
        // Act
        var result = Role.Create(roleName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(roleName);
    }

    #endregion

    #region AddPermission Tests

    /// <summary>
    /// Verifies that AddPermission successfully adds a new permission with Allow effect.
    /// </summary>
    [Fact]
    public void AddPermission_WithAllowEffect_ShouldReturnSuccess()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permission = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Act
        var result = role.AddPermission(permission);

        // Assert
        result.IsSuccess.Should().BeTrue();
        role.Permissions.Should().ContainSingle();
        role.Permissions.Should().Contain(permission);
    }

    /// <summary>
    /// Verifies that AddPermission returns failure when permission has Deny effect.
    /// </summary>
    [Fact]
    public void AddPermission_WithDenyEffect_ShouldReturnFailure()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permission = new Permission(
            EffectType.Deny,
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Act
        var result = role.AddPermission(permission);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Role.InvalidPermissionEffect");
        role.Permissions.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that AddPermission returns failure when permission already exists.
    /// </summary>
    [Fact]
    public void AddPermission_WithDuplicatePermission_ShouldReturnFailure()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permission = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        role.AddPermission(permission);

        // Act
        var result = role.AddPermission(permission);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PermissionErrors.PermissionAlreadyAssigned);
        role.Permissions.Should().ContainSingle();
    }

    /// <summary>
    /// Verifies that AddPermission raises RolePermissionAssignedDomainEvent.
    /// </summary>
    [Fact]
    public void AddPermission_ShouldRaiseRolePermissionAssignedDomainEvent()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        role.ClearDomainEvents();
        var permission = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Act
        role.AddPermission(permission);

        // Assert
        role.DomainEvents.Should().ContainSingle(e => e is RolePermissionAddedDomainEvent);
        var domainEvent = role.DomainEvents.OfType<RolePermissionAddedDomainEvent>().First();
        domainEvent.Role.Should().Be(role);
        domainEvent.Permission.Should().Be(permission);
    }

    /// <summary>
    /// Verifies that AddPermission can add multiple different permissions.
    /// </summary>
    [Fact]
    public void AddPermission_WithMultiplePermissions_ShouldAddAll()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permission1 = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        var permission2 = Permission.CreateRolePermission(
            ActionType.Update,
            ResourceType.User,
            ResourceId.Create("user-456"));
        var permission3 = Permission.CreateRolePermission(
            ActionType.Delete,
            ResourceType.Lesson,
            ResourceId.Create("lesson-789"));

        // Act
        role.AddPermission(permission1);
        role.AddPermission(permission2);
        role.AddPermission(permission3);

        // Assert
        role.Permissions.Should().HaveCount(3);
        role.Permissions.Should().Contain(new[] { permission1, permission2, permission3 });
    }

    /// <summary>
    /// Verifies that AddPermission works with wildcard permissions.
    /// </summary>
    [Fact]
    public void AddPermission_WithWildcardPermission_ShouldReturnSuccess()
    {
        // Arrange
        var role = Role.Create("SuperAdmin").Value;
        var permission = Permission.CreateRolePermission(
            ActionType.Wildcard,
            ResourceType.Wildcard,
            ResourceId.Wildcard);

        // Act
        var result = role.AddPermission(permission);

        // Assert
        result.IsSuccess.Should().BeTrue();
        role.Permissions.Should().ContainSingle();
        role.Permissions.Should().Contain(permission);
    }

    #endregion

    #region RemovePermission Tests

    /// <summary>
    /// Verifies that RemovePermission successfully removes an existing permission.
    /// </summary>
    [Fact]
    public void RemovePermission_WithExistingPermission_ShouldReturnSuccess()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permission = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        role.AddPermission(permission);

        // Act
        var result = role.RemovePermission(permission);

        // Assert
        result.IsSuccess.Should().BeTrue();
        role.Permissions.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that RemovePermission returns success even when permission doesn't exist.
    /// </summary>
    [Fact]
    public void RemovePermission_WithNonExistingPermission_ShouldReturnSuccess()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permission = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Act
        var result = role.RemovePermission(permission);

        // Assert
        result.IsSuccess.Should().BeTrue();
        role.Permissions.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that RemovePermission raises RolePermissionRemovedDomainEvent when permission exists.
    /// </summary>
    [Fact]
    public void RemovePermission_WithExistingPermission_ShouldRaiseRolePermissionRemovedDomainEvent()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permission = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        role.AddPermission(permission);
        role.ClearDomainEvents();

        // Act
        role.RemovePermission(permission);

        // Assert
        role.DomainEvents.Should().ContainSingle(e => e is RolePermissionRemovedDomainEvent);
        var domainEvent = role.DomainEvents.OfType<RolePermissionRemovedDomainEvent>().First();
        domainEvent.Role.Should().Be(role);
        domainEvent.Permission.Should().Be(permission);
    }

    /// <summary>
    /// Verifies that RemovePermission doesn't raise event when permission doesn't exist.
    /// </summary>
    [Fact]
    public void RemovePermission_WithNonExistingPermission_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        role.ClearDomainEvents();
        var permission = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Act
        role.RemovePermission(permission);

        // Assert
        role.DomainEvents.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that RemovePermission can remove specific permission from multiple permissions.
    /// </summary>
    [Fact]
    public void RemovePermission_WithMultiplePermissions_ShouldRemoveOnlySpecified()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permission1 = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        var permission2 = Permission.CreateRolePermission(
            ActionType.Update,
            ResourceType.User,
            ResourceId.Create("user-456"));
        role.AddPermission(permission1);
        role.AddPermission(permission2);

        // Act
        role.RemovePermission(permission1);

        // Assert
        role.Permissions.Should().ContainSingle();
        role.Permissions.Should().Contain(permission2);
        role.Permissions.Should().NotContain(permission1);
    }

    #endregion

    #region Permissions Collection Tests

    /// <summary>
    /// Verifies that Permissions property returns read-only collection.
    /// </summary>
    [Fact]
    public void Permissions_ShouldBeReadOnly()
    {
        // Arrange
        var role = Role.Create("Admin").Value;

        // Act
        var permissions = role.Permissions;

        // Assert
        permissions.Should().BeAssignableTo<IReadOnlyCollection<Permission>>();
    }

    /// <summary>
    /// Verifies that newly created role has no permissions.
    /// </summary>
    [Fact]
    public void Permissions_OnNewRole_ShouldBeEmpty()
    {
        // Arrange & Act
        var role = Role.Create("Admin").Value;

        // Assert
        role.Permissions.Should().BeEmpty();
    }

    #endregion
}
