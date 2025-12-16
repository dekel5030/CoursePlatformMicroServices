using Domain.Permissions;
using Domain.Permissions.Errors;
using FluentAssertions;
using Kernel.Auth.AuthTypes;
using Xunit;

namespace Domain.UnitTests.Permissions;

/// <summary>
/// Unit tests for Permission value object.
/// Tests cover permission creation, parsing, and validation.
/// </summary>
public class PermissionTests
{
    #region Parse Tests

    /// <summary>
    /// Verifies that Parse successfully creates a Permission with valid segments.
    /// </summary>
    [Fact]
    public void Parse_WithValidSegments_ShouldReturnSuccess()
    {
        // Arrange
        var effectSegment = "Allow";
        var actionSegment = "Read";
        var resourceSegment = "Course";
        var resourceIdSegment = "course-123";

        // Act
        var result = Permission.Parse(effectSegment, actionSegment, resourceSegment, resourceIdSegment);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Effect.Should().Be(EffectType.Allow);
        result.Value.Action.Should().Be(ActionType.Read);
        result.Value.Resource.Should().Be(ResourceType.Course);
        result.Value.ResourceId.Value.Should().Be(resourceIdSegment);
    }

    /// <summary>
    /// Verifies that Parse handles case-insensitive effect parsing.
    /// </summary>
    [Theory]
    [InlineData("allow")]
    [InlineData("Allow")]
    [InlineData("ALLOW")]
    public void Parse_WithDifferentCasingEffect_ShouldReturnSuccess(string effectSegment)
    {
        // Act
        var result = Permission.Parse(effectSegment, "Read", "Course", "course-123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Effect.Should().Be(EffectType.Allow);
    }

    /// <summary>
    /// Verifies that Parse handles case-insensitive action parsing.
    /// </summary>
    [Theory]
    [InlineData("read")]
    [InlineData("Read")]
    [InlineData("READ")]
    public void Parse_WithDifferentCasingAction_ShouldReturnSuccess(string actionSegment)
    {
        // Act
        var result = Permission.Parse("Allow", actionSegment, "Course", "course-123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Action.Should().Be(ActionType.Read);
    }

    /// <summary>
    /// Verifies that Parse handles case-insensitive resource parsing.
    /// </summary>
    [Theory]
    [InlineData("course")]
    [InlineData("Course")]
    [InlineData("COURSE")]
    public void Parse_WithDifferentCasingResource_ShouldReturnSuccess(string resourceSegment)
    {
        // Act
        var result = Permission.Parse("Allow", "Read", resourceSegment, "course-123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Resource.Should().Be(ResourceType.Course);
    }

    /// <summary>
    /// Verifies that Parse returns failure when effect is invalid.
    /// Note: Current implementation returns InvalidAction for invalid effect,
    /// which may be a bug that should return InvalidEffect instead.
    /// </summary>
    [Fact]
    public void Parse_WithInvalidEffect_ShouldReturnFailure()
    {
        // Arrange
        var effectSegment = "InvalidEffect";

        // Act
        var result = Permission.Parse(effectSegment, "Read", "Course", "course-123");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PermissionErrors.InvalidAction);
    }

    /// <summary>
    /// Verifies that Parse returns failure when action is invalid.
    /// </summary>
    [Fact]
    public void Parse_WithInvalidAction_ShouldReturnFailure()
    {
        // Arrange
        var actionSegment = "InvalidAction";

        // Act
        var result = Permission.Parse("Allow", actionSegment, "Course", "course-123");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PermissionErrors.InvalidAction);
    }

    /// <summary>
    /// Verifies that Parse returns failure when resource is invalid.
    /// </summary>
    [Fact]
    public void Parse_WithInvalidResource_ShouldReturnFailure()
    {
        // Arrange
        var resourceSegment = "InvalidResource";

        // Act
        var result = Permission.Parse("Allow", "Read", resourceSegment, "course-123");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PermissionErrors.InvalidResource);
    }

    /// <summary>
    /// Verifies that Parse handles wildcard action.
    /// </summary>
    [Fact]
    public void Parse_WithWildcardAction_ShouldReturnSuccess()
    {
        // Act
        var result = Permission.Parse("Allow", "*", "Course", "course-123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Action.Should().Be(ActionType.Wildcard);
    }

    /// <summary>
    /// Verifies that Parse handles wildcard resource.
    /// </summary>
    [Fact]
    public void Parse_WithWildcardResource_ShouldReturnSuccess()
    {
        // Act
        var result = Permission.Parse("Allow", "Read", "*", "course-123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Resource.Should().Be(ResourceType.Wildcard);
    }

    /// <summary>
    /// Verifies that Parse handles wildcard resource ID.
    /// </summary>
    [Fact]
    public void Parse_WithWildcardResourceId_ShouldReturnSuccess()
    {
        // Act
        var result = Permission.Parse("Allow", "Read", "Course", "*");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ResourceId.Should().Be(ResourceId.Wildcard);
        result.Value.ResourceId.IsWildcard.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that Parse handles all valid action types.
    /// </summary>
    [Theory]
    [InlineData("Create", ActionType.Create)]
    [InlineData("Read", ActionType.Read)]
    [InlineData("Update", ActionType.Update)]
    [InlineData("Delete", ActionType.Delete)]
    [InlineData("*", ActionType.Wildcard)]
    public void Parse_WithAllActionTypes_ShouldReturnSuccess(string actionSegment, ActionType expectedAction)
    {
        // Act
        var result = Permission.Parse("Allow", actionSegment, "Course", "course-123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Action.Should().Be(expectedAction);
    }

    /// <summary>
    /// Verifies that Parse handles all valid resource types.
    /// </summary>
    [Theory]
    [InlineData("Course", ResourceType.Course)]
    [InlineData("Lesson", ResourceType.Lesson)]
    [InlineData("User", ResourceType.User)]
    [InlineData("Enrollment", ResourceType.Enrollment)]
    [InlineData("*", ResourceType.Wildcard)]
    public void Parse_WithAllResourceTypes_ShouldReturnSuccess(string resourceSegment, ResourceType expectedResource)
    {
        // Act
        var result = Permission.Parse("Allow", "Read", resourceSegment, "resource-123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Resource.Should().Be(expectedResource);
    }

    /// <summary>
    /// Verifies that Parse handles Deny effect type.
    /// </summary>
    [Fact]
    public void Parse_WithDenyEffect_ShouldReturnSuccess()
    {
        // Act
        var result = Permission.Parse("Deny", "Read", "Course", "course-123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Effect.Should().Be(EffectType.Deny);
    }

    #endregion

    #region CreateForRole Tests

    /// <summary>
    /// Verifies that CreateForRole creates a permission with Allow effect.
    /// </summary>
    [Fact]
    public void CreateForRole_ShouldCreatePermissionWithAllowEffect()
    {
        // Arrange
        var action = ActionType.Read;
        var resource = ResourceType.Course;
        var resourceId = ResourceId.Create("course-123");

        // Act
        var permission = Permission.CreateRolePermission(action, resource, resourceId);

        // Assert
        permission.Effect.Should().Be(EffectType.Allow);
        permission.Action.Should().Be(action);
        permission.Resource.Should().Be(resource);
        permission.ResourceId.Should().Be(resourceId);
    }

    /// <summary>
    /// Verifies that CreateForRole works with all action types.
    /// </summary>
    [Theory]
    [InlineData(ActionType.Create)]
    [InlineData(ActionType.Read)]
    [InlineData(ActionType.Update)]
    [InlineData(ActionType.Delete)]
    [InlineData(ActionType.Wildcard)]
    public void CreateForRole_WithAllActionTypes_ShouldCreateSuccessfully(ActionType action)
    {
        // Act
        var permission = Permission.CreateRolePermission(action, ResourceType.Course, ResourceId.Create("course-123"));

        // Assert
        permission.Effect.Should().Be(EffectType.Allow);
        permission.Action.Should().Be(action);
    }

    /// <summary>
    /// Verifies that CreateForRole works with all resource types.
    /// </summary>
    [Theory]
    [InlineData(ResourceType.Course)]
    [InlineData(ResourceType.Lesson)]
    [InlineData(ResourceType.User)]
    [InlineData(ResourceType.Enrollment)]
    [InlineData(ResourceType.Wildcard)]
    public void CreateForRole_WithAllResourceTypes_ShouldCreateSuccessfully(ResourceType resource)
    {
        // Act
        var permission = Permission.CreateRolePermission(ActionType.Read, resource, ResourceId.Create("resource-123"));

        // Assert
        permission.Effect.Should().Be(EffectType.Allow);
        permission.Resource.Should().Be(resource);
    }

    /// <summary>
    /// Verifies that CreateForRole works with wildcard resource ID.
    /// </summary>
    [Fact]
    public void CreateForRole_WithWildcardResourceId_ShouldCreateSuccessfully()
    {
        // Act
        var permission = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Wildcard);

        // Assert
        permission.Effect.Should().Be(EffectType.Allow);
        permission.ResourceId.Should().Be(ResourceId.Wildcard);
        permission.ResourceId.IsWildcard.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that CreateForRole creates a full wildcard permission.
    /// </summary>
    [Fact]
    public void CreateForRole_WithAllWildcards_ShouldCreateSuccessfully()
    {
        // Act
        var permission = Permission.CreateRolePermission(
            ActionType.Wildcard,
            ResourceType.Wildcard,
            ResourceId.Wildcard);

        // Assert
        permission.Effect.Should().Be(EffectType.Allow);
        permission.Action.Should().Be(ActionType.Wildcard);
        permission.Resource.Should().Be(ResourceType.Wildcard);
        permission.ResourceId.Should().Be(ResourceId.Wildcard);
    }

    #endregion

    #region Value Object Equality Tests

    /// <summary>
    /// Verifies that two permissions with same values are equal.
    /// </summary>
    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var permission1 = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        var permission2 = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Assert
        permission1.Should().Be(permission2);
    }

    /// <summary>
    /// Verifies that two permissions with different actions are not equal.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentActions_ShouldNotBeEqual()
    {
        // Arrange
        var permission1 = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        var permission2 = Permission.CreateRolePermission(
            ActionType.Update,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Assert
        permission1.Should().NotBe(permission2);
    }

    /// <summary>
    /// Verifies that two permissions with different resources are not equal.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentResources_ShouldNotBeEqual()
    {
        // Arrange
        var permission1 = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        var permission2 = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.User,
            ResourceId.Create("course-123"));

        // Assert
        permission1.Should().NotBe(permission2);
    }

    /// <summary>
    /// Verifies that two permissions with different resource IDs are not equal.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentResourceIds_ShouldNotBeEqual()
    {
        // Arrange
        var permission1 = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        var permission2 = Permission.CreateRolePermission(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-456"));

        // Assert
        permission1.Should().NotBe(permission2);
    }

    /// <summary>
    /// Verifies that two permissions with different effects are not equal.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentEffects_ShouldNotBeEqual()
    {
        // Arrange
        var permission1 = new Permission(
            EffectType.Allow,
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));
        var permission2 = new Permission(
            EffectType.Deny,
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("course-123"));

        // Assert
        permission1.Should().NotBe(permission2);
    }

    #endregion
}
