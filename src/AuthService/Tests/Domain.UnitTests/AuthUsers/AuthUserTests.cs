using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.AuthUsers.Events;
using Domain.Permissions;
using Domain.Roles;
using FluentAssertions;
using Kernel.Auth.AuthTypes;
using Xunit;

namespace Domain.UnitTests.AuthUsers;

/// <summary>
/// Unit tests for AuthUser domain entity.
/// Tests cover user creation, role management, permission management, and domain events.
/// </summary>
public class AuthUserTests
{
    private readonly Role _defaultRole;

    public AuthUserTests()
    {
        _defaultRole = Role.Create("User").Value;
    }

    #region Create Tests

    /// <summary>
    /// Verifies that Create successfully creates an AuthUser with valid data.
    /// </summary>
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "testuser";

        // Act
        var result = AuthUser.Create(email, userName, _defaultRole);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be(email);
        result.Value.UserName.Should().Be(userName);
        result.Value.Id.Should().NotBeEmpty();
    }

    /// <summary>
    /// Verifies that Create assigns the initial role to the user.
    /// </summary>
    [Fact]
    public void Create_ShouldAssignInitialRole()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "testuser";

        // Act
        var result = AuthUser.Create(email, userName, _defaultRole);

        // Assert
        result.Value.Roles.Should().ContainSingle();
        result.Value.Roles.First().Should().Be(_defaultRole);
    }

    /// <summary>
    /// Verifies that Create uses email as username when username is empty.
    /// </summary>
    [Fact]
    public void Create_WithEmptyUserName_ShouldUseEmailAsUserName()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var result = AuthUser.Create(email, string.Empty, _defaultRole);

        // Assert
        result.Value.UserName.Should().Be(email);
    }

    /// <summary>
    /// Verifies that Create uses email as username when username is null.
    /// </summary>
    [Fact]
    public void Create_WithNullUserName_ShouldUseEmailAsUserName()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var result = AuthUser.Create(email, null!, _defaultRole);

        // Assert
        result.Value.UserName.Should().Be(email);
    }

    /// <summary>
    /// Verifies that Create raises UserRegisteredDomainEvent.
    /// </summary>
    [Fact]
    public void Create_ShouldRaiseUserRegisteredDomainEvent()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "testuser";

        // Act
        var result = AuthUser.Create(email, userName, _defaultRole);

        // Assert
        result.Value.DomainEvents.Should().ContainSingle(e => e is UserRegisteredDomainEvent);
        var domainEvent = result.Value.DomainEvents.OfType<UserRegisteredDomainEvent>().First();
        domainEvent.User.Should().Be(result.Value);
    }

    /// <summary>
    /// Verifies that Create raises UserRoleAddedDomainEvent for initial role.
    /// </summary>
    [Fact]
    public void Create_ShouldRaiseUserRoleAddedDomainEvent()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "testuser";

        // Act
        var result = AuthUser.Create(email, userName, _defaultRole);

        // Assert
        result.Value.DomainEvents.Should().Contain(e => e is UserRoleAddedDomainEvent);
        var domainEvent = result.Value.DomainEvents.OfType<UserRoleAddedDomainEvent>().First();
        domainEvent.User.Should().Be(result.Value);
        domainEvent.Role.Should().Be(_defaultRole);
    }

    /// <summary>
    /// Verifies that Create generates a unique ID for each user.
    /// </summary>
    [Fact]
    public void Create_ShouldGenerateUniqueId()
    {
        // Arrange & Act
        var user1 = AuthUser.Create("user1@example.com", "user1", _defaultRole).Value;
        var user2 = AuthUser.Create("user2@example.com", "user2", _defaultRole).Value;

        // Assert
        user1.Id.Should().NotBe(user2.Id);
    }

    #endregion

    #region AddRole Tests

    /// <summary>
    /// Verifies that AddRole successfully adds a new role to the user.
    /// </summary>
    [Fact]
    public void AddRole_WithNewRole_ShouldReturnSuccess()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var adminRole = Role.Create("Admin").Value;

        // Act
        var result = user.AddRole(adminRole);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Roles.Should().HaveCount(2);
        user.Roles.Should().Contain(adminRole);
    }

    /// <summary>
    /// Verifies that AddRole returns failure when role already exists.
    /// </summary>
    [Fact]
    public void AddRole_WithExistingRole_ShouldReturnFailure()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;

        // Act
        var result = user.AddRole(_defaultRole);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthUserErrors.RoleAlreadyExists);
        user.Roles.Should().ContainSingle();
    }

    /// <summary>
    /// Verifies that AddRole raises UserRoleAddedDomainEvent.
    /// </summary>
    [Fact]
    public void AddRole_ShouldRaiseUserRoleAddedDomainEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        user.ClearDomainEvents();
        var adminRole = Role.Create("Admin").Value;

        // Act
        user.AddRole(adminRole);

        // Assert
        user.DomainEvents.Should().ContainSingle(e => e is UserRoleAddedDomainEvent);
        var domainEvent = user.DomainEvents.OfType<UserRoleAddedDomainEvent>().First();
        domainEvent.User.Should().Be(user);
        domainEvent.Role.Should().Be(adminRole);
    }

    /// <summary>
    /// Verifies that AddRole can add multiple different roles.
    /// </summary>
    [Fact]
    public void AddRole_WithMultipleRoles_ShouldAddAll()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var adminRole = Role.Create("Admin").Value;
        var moderatorRole = Role.Create("Moderator").Value;

        // Act
        user.AddRole(adminRole);
        user.AddRole(moderatorRole);

        // Assert
        user.Roles.Should().HaveCount(3);
        user.Roles.Should().Contain(new[] { _defaultRole, adminRole, moderatorRole });
    }

    #endregion

    #region RemoveRole Tests

    /// <summary>
    /// Verifies that RemoveRole successfully removes an existing role.
    /// </summary>
    [Fact]
    public void RemoveRole_WithExistingRole_ShouldReturnSuccess()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var adminRole = Role.Create("Admin").Value;
        user.AddRole(adminRole);

        // Act
        var result = user.RemoveRole(adminRole);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Roles.Should().ContainSingle();
        user.Roles.Should().NotContain(adminRole);
    }

    /// <summary>
    /// Verifies that RemoveRole returns success even when role doesn't exist.
    /// </summary>
    [Fact]
    public void RemoveRole_WithNonExistingRole_ShouldReturnSuccess()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var adminRole = Role.Create("Admin").Value;

        // Act
        var result = user.RemoveRole(adminRole);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Roles.Should().ContainSingle();
    }

    /// <summary>
    /// Verifies that RemoveRole raises UserRoleRemovedDomainEvent when role exists.
    /// </summary>
    [Fact]
    public void RemoveRole_WithExistingRole_ShouldRaiseUserRoleRemovedDomainEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var adminRole = Role.Create("Admin").Value;
        user.AddRole(adminRole);
        user.ClearDomainEvents();

        // Act
        user.RemoveRole(adminRole);

        // Assert
        user.DomainEvents.Should().ContainSingle(e => e is UserRoleRemovedDomainEvent);
        var domainEvent = user.DomainEvents.OfType<UserRoleRemovedDomainEvent>().First();
        domainEvent.User.Should().Be(user);
        domainEvent.Role.Should().Be(adminRole);
    }

    /// <summary>
    /// Verifies that RemoveRole doesn't raise event when role doesn't exist.
    /// </summary>
    [Fact]
    public void RemoveRole_WithNonExistingRole_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        user.ClearDomainEvents();
        var adminRole = Role.Create("Admin").Value;

        // Act
        user.RemoveRole(adminRole);

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region AddPermission Tests

    /// <summary>
    /// Verifies that AddPermission successfully adds a new permission to the user.
    /// </summary>
    [Fact]
    public void AddPermission_WithNewPermission_ShouldReturnSuccess()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Act
        var result = user.AddPermission(permission);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Permissions.Should().ContainSingle();
        user.Permissions.Should().Contain(permission);
    }

    /// <summary>
    /// Verifies that AddPermission returns failure when permission already exists.
    /// </summary>
    [Fact]
    public void AddPermission_WithExistingPermission_ShouldReturnFailure()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        user.AddPermission(permission);

        // Act
        var result = user.AddPermission(permission);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AuthUser.PermissionAlreadyExists");
        user.Permissions.Should().ContainSingle();
    }

    /// <summary>
    /// Verifies that AddPermission raises UserPermissionAddedDomainEvent.
    /// </summary>
    [Fact]
    public void AddPermission_ShouldRaiseUserPermissionAddedDomainEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        user.ClearDomainEvents();
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Act
        user.AddPermission(permission);

        // Assert
        user.DomainEvents.Should().ContainSingle(e => e is UserPermissionAddedDomainEvent);
        var domainEvent = user.DomainEvents.OfType<UserPermissionAddedDomainEvent>().First();
        domainEvent.User.Should().Be(user);
        domainEvent.Permission.Should().Be(permission);
    }

    /// <summary>
    /// Verifies that AddPermission can add multiple different permissions.
    /// </summary>
    [Fact]
    public void AddPermission_WithMultiplePermissions_ShouldAddAll()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permission1 = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        var permission2 = Permission.CreateForRole(
            ActionType.Update,
            ResourceType.User,
            ResourceId.Create("user-456"));

        // Act
        user.AddPermission(permission1);
        user.AddPermission(permission2);

        // Assert
        user.Permissions.Should().HaveCount(2);
        user.Permissions.Should().Contain(new[] { permission1, permission2 });
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
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        user.AddPermission(permission);

        // Act
        var result = user.RemovePermission(permission);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Permissions.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that RemovePermission returns success even when permission doesn't exist.
    /// </summary>
    [Fact]
    public void RemovePermission_WithNonExistingPermission_ShouldReturnSuccess()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Act
        var result = user.RemovePermission(permission);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Permissions.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that RemovePermission raises UserPermissionRemovedDomainEvent when permission exists.
    /// </summary>
    [Fact]
    public void RemovePermission_WithExistingPermission_ShouldRaiseUserPermissionRemovedDomainEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        user.AddPermission(permission);
        user.ClearDomainEvents();

        // Act
        user.RemovePermission(permission);

        // Assert
        user.DomainEvents.Should().ContainSingle(e => e is UserPermissionRemovedDomainEvent);
        var domainEvent = user.DomainEvents.OfType<UserPermissionRemovedDomainEvent>().First();
        domainEvent.User.Should().Be(user);
        domainEvent.Permission.Should().Be(permission);
    }

    /// <summary>
    /// Verifies that RemovePermission doesn't raise event when permission doesn't exist.
    /// </summary>
    [Fact]
    public void RemovePermission_WithNonExistingPermission_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        user.ClearDomainEvents();
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Act
        user.RemovePermission(permission);

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region AddPermissions (Batch) Tests

    /// <summary>
    /// Verifies that AddPermissions successfully adds multiple permissions at once.
    /// </summary>
    [Fact]
    public void AddPermissions_WithMultipleValidPermissions_ShouldReturnSuccess()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permissions = new[]
        {
            Permission.CreateForRole(ActionType.Read, ResourceType.Course, ResourceId.Create("course-123")),
            Permission.CreateForRole(ActionType.Update, ResourceType.User, ResourceId.Create("user-456")),
            Permission.CreateForRole(ActionType.Delete, ResourceType.Lesson, ResourceId.Create("lesson-789"))
        };

        // Act
        var result = user.AddPermissions(permissions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Permissions.Should().HaveCount(3);
        user.Permissions.Should().Contain(permissions);
    }

    /// <summary>
    /// Verifies that AddPermissions raises a single UserPermissionsUpdatedDomainEvent.
    /// </summary>
    [Fact]
    public void AddPermissions_ShouldRaiseSingleUserPermissionsUpdatedDomainEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        user.ClearDomainEvents();
        var permissions = new[]
        {
            Permission.CreateForRole(ActionType.Read, ResourceType.Course, ResourceId.Create("course-123")),
            Permission.CreateForRole(ActionType.Update, ResourceType.User, ResourceId.Create("user-456")),
            Permission.CreateForRole(ActionType.Delete, ResourceType.Lesson, ResourceId.Create("lesson-789"))
        };

        // Act
        user.AddPermissions(permissions);

        // Assert
        user.DomainEvents.Should().ContainSingle(e => e is UserPermissionsUpdatedDomainEvent);
        var domainEvent = user.DomainEvents.OfType<UserPermissionsUpdatedDomainEvent>().First();
        domainEvent.User.Should().Be(user);
        domainEvent.AddedPermissions.Should().HaveCount(3);
        domainEvent.RemovedPermissions.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that AddPermissions skips duplicate permissions.
    /// </summary>
    [Fact]
    public void AddPermissions_WithDuplicatePermissions_ShouldSkipDuplicates()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permission1 = Permission.CreateForRole(ActionType.Read, ResourceType.Course, ResourceId.Create("course-123"));
        user.AddPermission(permission1);
        user.ClearDomainEvents();

        var permissions = new[]
        {
            permission1, // duplicate
            Permission.CreateForRole(ActionType.Update, ResourceType.User, ResourceId.Create("user-456"))
        };

        // Act
        var result = user.AddPermissions(permissions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Permissions.Should().HaveCount(2);
        var domainEvent = user.DomainEvents.OfType<UserPermissionsUpdatedDomainEvent>().FirstOrDefault();
        domainEvent?.AddedPermissions.Should().HaveCount(1);
    }

    /// <summary>
    /// Verifies that AddPermissions doesn't raise event when no permissions are added.
    /// </summary>
    [Fact]
    public void AddPermissions_WithAllDuplicates_ShouldNotRaiseEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permission = Permission.CreateForRole(ActionType.Read, ResourceType.Course, ResourceId.Create("course-123"));
        user.AddPermission(permission);
        user.ClearDomainEvents();

        // Act
        user.AddPermissions(new[] { permission });

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region RemovePermissions (Batch) Tests

    /// <summary>
    /// Verifies that RemovePermissions successfully removes multiple permissions at once.
    /// </summary>
    [Fact]
    public void RemovePermissions_WithMultipleExistingPermissions_ShouldReturnSuccess()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permissions = new[]
        {
            Permission.CreateForRole(ActionType.Read, ResourceType.Course, ResourceId.Create("course-123")),
            Permission.CreateForRole(ActionType.Update, ResourceType.User, ResourceId.Create("user-456")),
            Permission.CreateForRole(ActionType.Delete, ResourceType.Lesson, ResourceId.Create("lesson-789"))
        };
        user.AddPermissions(permissions);
        user.ClearDomainEvents();

        // Act
        var result = user.RemovePermissions(permissions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Permissions.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that RemovePermissions raises a single UserPermissionsUpdatedDomainEvent.
    /// </summary>
    [Fact]
    public void RemovePermissions_ShouldRaiseSingleUserPermissionsUpdatedDomainEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permissions = new[]
        {
            Permission.CreateForRole(ActionType.Read, ResourceType.Course, ResourceId.Create("course-123")),
            Permission.CreateForRole(ActionType.Update, ResourceType.User, ResourceId.Create("user-456"))
        };
        user.AddPermissions(permissions);
        user.ClearDomainEvents();

        // Act
        user.RemovePermissions(permissions);

        // Assert
        user.DomainEvents.Should().ContainSingle(e => e is UserPermissionsUpdatedDomainEvent);
        var domainEvent = user.DomainEvents.OfType<UserPermissionsUpdatedDomainEvent>().First();
        domainEvent.User.Should().Be(user);
        domainEvent.RemovedPermissions.Should().HaveCount(2);
        domainEvent.AddedPermissions.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that RemovePermissions skips non-existing permissions.
    /// </summary>
    [Fact]
    public void RemovePermissions_WithNonExistingPermissions_ShouldSkipNonExisting()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        var permission1 = Permission.CreateForRole(ActionType.Read, ResourceType.Course, ResourceId.Create("course-123"));
        user.AddPermission(permission1);
        user.ClearDomainEvents();

        var permissions = new[]
        {
            permission1,
            Permission.CreateForRole(ActionType.Update, ResourceType.User, ResourceId.Create("user-456")) // doesn't exist
        };

        // Act
        var result = user.RemovePermissions(permissions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Permissions.Should().BeEmpty();
        var domainEvent = user.DomainEvents.OfType<UserPermissionsUpdatedDomainEvent>().FirstOrDefault();
        domainEvent?.RemovedPermissions.Should().HaveCount(1);
    }

    /// <summary>
    /// Verifies that RemovePermissions doesn't raise event when no permissions are removed.
    /// </summary>
    [Fact]
    public void RemovePermissions_WithAllNonExisting_ShouldNotRaiseEvent()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;
        user.ClearDomainEvents();
        var permissions = new[]
        {
            Permission.CreateForRole(ActionType.Read, ResourceType.Course, ResourceId.Create("course-123"))
        };

        // Act
        user.RemovePermissions(permissions);

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region Roles Collection Tests

    /// <summary>
    /// Verifies that Roles property returns read-only collection.
    /// </summary>
    [Fact]
    public void Roles_ShouldBeReadOnly()
    {
        // Arrange
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;

        // Act
        var roles = user.Roles;

        // Assert
        roles.Should().BeAssignableTo<IReadOnlyCollection<Role>>();
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
        var user = AuthUser.Create("test@example.com", "testuser", _defaultRole).Value;

        // Act
        var permissions = user.Permissions;

        // Assert
        permissions.Should().BeAssignableTo<IReadOnlyCollection<Permission>>();
    }

    #endregion
}
